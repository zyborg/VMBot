using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Zyborg.AWS.Lambda
{
    public class MultiEventFunction<TFunction, TResult> : LambdaFunction<TFunction>
        where TFunction : MultiEventFunction<TFunction, TResult>
    {
        private static readonly Type[] MatcherMethodArgTypes = new[] { typeof(JsonDocument) };

        private ILogger _logger;
        private LambdaJsonEventDecoder<TResult> _decoder;
        private MethodInfo _defaultHandler;

        protected MultiEventFunction()
        { }

        protected override void PrepareServices(IServiceCollection services, IConfiguration config)
        {
            base.PrepareServices(services, config);

            services.AddSingleton<LambdaJsonEventDecoder<TResult>>();

            services.AddPrePrepareFinalAction(services =>
            {
                _logger =  services.GetRequiredService<ILogger<MultiEventFunction<TFunction, TResult>>>();
                _decoder = services.GetRequiredService<LambdaJsonEventDecoder<TResult>>();
            });

            services.AddPostPrepareFinalAction(services =>
            {
                RegisterHandlers();
            });
        }

        public Task<Stream> MultiEventFunctionHandler(Stream input, ILambdaContext context)
        {
            _logger.LogInformation("Receiving input from main entry point invocation");

            return _decoder.DecodeEventAsync(input, context);
        }

        public void Register<TEvent>(LambdaJsonEventDecoder<TResult>.LambdaEventMatcher matcher)
        {
            _logger.LogInformation($"Registering Matcher for Event Type [{typeof(TEvent).FullName}]");

            _decoder.AppendEventTypeMatcher(typeof(TEvent), matcher);
        }

        public void RegisterCommonEventMatchers(IEnumerable<Type> includeOnly = null)
        {
            _logger.LogInformation("Registering common Matcher");

            _decoder.AppendCommonMatchers(includeOnly);
        }

        private void RegisterHandlers()
        {
            _logger.LogInformation("Registering Handlers from attributed methods");

            var thisType = this.GetType();

            // Cycle through each public instance method and find those that are
            // attributed to handle an AWS Lambda Event or custom matching logic
            foreach (var m in thisType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                var fhAtt = m.GetCustomAttribute<LambdaEventHandlerAttribute>();
                if (fhAtt == null)
                    continue;
                
                if (fhAtt.MatcherMethod != null)
                {
                    // If a matcher method name was specified make sure it exists as
                    // a public instance method with the appropriate type signature
                    var matcherMethod = thisType.GetMethod(fhAtt.MatcherMethod,
                        BindingFlags.Public | BindingFlags.Instance,
                        callConvention: CallingConventions.HasThis,
                        types: MatcherMethodArgTypes,
                        binder: null,
                        modifiers: null);
                    if (matcherMethod == null)
                    {
                        throw new Exception("Matcher method for Lambda Event Handler not found: "
                            + fhAtt.MatcherMethod);
                    }

                    _decoder.AppendHandler(
                        // Matcher
                        (jtoken) => (bool)matcherMethod.Invoke(this, new[] { jtoken }),
                        // Handler
                        CreateHandler(m)
                    );
                }
                else if (fhAtt.EventType != null)
                {
                    // If an event type was specified, we register the handler and _assume_ the
                    // event type matcher has already been registered, e.g. in the 'ctor of this class
                    _decoder.SetHandler(fhAtt.EventType, CreateHandler(m));
                }
                else
                {
                    // Otherwise we _assume_ this is the *defult* handler -- there can be only one!
                    if (!fhAtt.IsDefaultHandler)
                    {
                        throw new Exception("Lambda Event Handler without matcher or event type is unexpectedly a Default Handler");
                    }
                    if (_defaultHandler != null)
                    {
                        throw new Exception("Default Handler already defined; there can be only one");
                    }
                    _defaultHandler = m;
                    _decoder.SetDefaultHandler(CreateHandler(m));
                }
            }
        }

        private LambdaJsonEventDecoder<TResult>.LambdaEventHandler CreateHandler(MethodInfo m)
        {
            var handlerMethodParams = m.GetParameters();

            // Make sure the method has a valid signature
            if (handlerMethodParams.Length > 0)
            {
                if (handlerMethodParams.Length > 1)
                {
                    if (!handlerMethodParams[1].ParameterType.IsAssignableFrom(typeof(ILambdaContext)))
                        throw new Exception("Event Handler method signature mismatch,"
                            + " parameter 2 must be assignable from Context: " + m.Name);
                    
                    if (handlerMethodParams.Length > 2)
                    {
                        if (!handlerMethodParams[1].ParameterType.IsAssignableFrom(typeof(object)))
                            throw new Exception("Event Handler method signature mismatch,"
                                + " parameter 3 must be assignable from parameter data Object: " + m.Name);

                        if (handlerMethodParams.Length > 3)
                            throw new Exception("Event Handler method parameter count mismatch: " + m.Name);
                    }
                }
            }

            // Return a handler that maps incoming params to the method's signature
            return (jtoken, ctx, data) => {
                var args = new object[handlerMethodParams.Length];

                if (handlerMethodParams.Length > 0)
                {
                    if (handlerMethodParams.Length > 1)
                    {
                        if (handlerMethodParams.Length > 2)
                        {
                            args[2] = data;
                        }

                        args[1] = ctx;
                    }

                    if (handlerMethodParams[0].ParameterType.IsAssignableFrom(typeof(JToken)))
                    {
                        args[0] = jtoken;
                    }
                    else if (handlerMethodParams[0].ParameterType.IsAssignableFrom(typeof(string)))
                    {
                        args[0] = jtoken.ToString();
                    }
                    else
                    {
                        args[0] = jtoken.ToObject(handlerMethodParams[0].ParameterType, _decoder.DefaultJsonSerializer);
                    }
                }
                return (Task<TResult>)m.Invoke(this, args);
            };
        }
    }
}