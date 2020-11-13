using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using LambdaJsonSerializer = Amazon.Lambda.Serialization.Json.JsonSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Zyborg.AWS.Lambda
{
    public class LambdaJsonEventDecoder<TResult>
    {
        public delegate bool LambdaEventMatcher(JToken jtoken);
        public delegate Task<TResult> LambdaEventHandler(JToken input, ILambdaContext context, object paramData);

        private ILogger _logger;

        private LambdaEventHandler _defaultHandler;
        private List<(LambdaEventMatcher matcher, LambdaEventHandler handler)> _matcherHandlers =
            new List<(LambdaEventMatcher, LambdaEventHandler)>();

        private List<string> _eventTypes = new List<string>();
        private Dictionary<string, LambdaEventMatcher> _eventTypeMatchers =
            new Dictionary<string, LambdaEventMatcher>();
        private Dictionary<string, LambdaEventHandler> _eventTypeHandlers =
            new Dictionary<string, LambdaEventHandler>();

        private static readonly Dictionary<string, LambdaEventMatcher> _commonEventTypeMatchers = new Dictionary<string, LambdaEventMatcher>
        {
            // This was assembled with the help of:
            //    https://docs.aws.amazon.com/lambda/latest/dg/eventsources.html
            // and the "in-box" event classes found here:
            //    https://github.com/aws/aws-lambda-dotnet/tree/master/Libraries/src

            ["Amazon.Lambda.S3Events.S3Event"] = (jtoken) =>
                jtoken.SelectToken("Records[0].s3") != null,
            ["Amazon.Lambda.SNSEvents.SNSEVent"] = (jtoken) =>
                jtoken.SelectToken("Records[0].Sns") != null,
            ["Amazon.Lambda.SQSEvents.SQSEvent"] = (jtoken) =>
                jtoken.SelectToken("Records[0].messageId") != null && jtoken.SelectToken("Records[0].body") != null,
            ["Amazon.Lambda.CloudWatchLogsEvents.CloudWatchLogsEvent"] = (jtoken) =>
                jtoken.SelectToken("awslogs") != null,
            // // This one is WRONG in the link above, but is clarified here:
            // //    https://docs.aws.amazon.com/ses/latest/DeveloperGuide/receiving-email-action-lambda.html
            ["Amazon.Lambda.SimpleEmailEvents.SimpleEmailEvent[1]"] = (jtoken) =>
                jtoken.SelectToken("Records[0].ses") != null,
            
            ["Amazon.Lambda.CloudWatchEvents<Zyborg.AWS.Lambda.EC2StateChangeDetail>"] =
                Events.EC2StateChangeDetail.Matcher,

        };

        private DecoderLambdaJsonSerializer _lambdaSer = new DecoderLambdaJsonSerializer();
        private Newtonsoft.Json.JsonSerializer _jsonSer;

        public Newtonsoft.Json.JsonSerializer DefaultJsonSerializer
        {
            get
            {
                if (_jsonSer == null)
                {
                    _jsonSer = Newtonsoft.Json.JsonSerializer.Create(_lambdaSer.DefaultSettings);
                }
                return _jsonSer;
            }
        }

        public LambdaJsonEventDecoder(ILogger<LambdaJsonEventDecoder<TResult>> logger)
        {
            _logger = (ILogger)logger ?? NullLogger.Instance;
        }

        public void ClearMatchers()
        {
            _eventTypes.Clear();
            _eventTypeMatchers.Clear();
        }

        public void ClearHandlers()
        {
            _defaultHandler = null;
            _matcherHandlers.Clear();
            _eventTypeHandlers.Clear();
        }

        public void SetDefaultHandler(LambdaEventHandler handler)
        {
            if (_defaultHandler != null)
            {
                throw new InvalidOperationException("default handler has already been registered");
            }
            _logger.LogInformation($"Setting Default Handler");
            _defaultHandler = handler;
        }

        public void AppendCommonMatchers(IEnumerable<Type> includeOnly = null)
        {
            var includeOnlyKeys = includeOnly == null
                ? null
                : includeOnly.ToDictionary(t => ComputeTypeKey(t), t => true);

            _logger.LogInformation("Appending common Event Type Matchers...");

            foreach (var (typeKey, matcher) in _commonEventTypeMatchers)
            {
                if (includeOnlyKeys == null || includeOnlyKeys.ContainsKey(typeKey))
                {
                    if (_eventTypeMatchers.ContainsKey(typeKey))
                    {
                        throw new InvalidOperationException(
                            "matcher for event type has already been registered: " + typeKey);
                    }
                    _logger.LogInformation($"Appending common Matcher for Event Type key [{typeKey}]");
                    _eventTypeMatchers.Add(typeKey, matcher);
                    _eventTypes.Add(typeKey);
                }
            }
        }

        public void AppendHandler(LambdaEventMatcher matcher, LambdaEventHandler handler)
        {
            _matcherHandlers.Add((matcher, handler));
        }

        public void InsertHandler(LambdaEventMatcher matcher, LambdaEventHandler handler)
        {
            _matcherHandlers.Insert(0, (matcher, handler));
        }

        private string ComputeTypeKey(Type t)
        {
            if (t.IsGenericTypeDefinition)
            {
                return $"{t.FullName}[{t.GetGenericArguments().Length}]";
            }

            if (t.IsGenericType)
            {
                return $"{t.FullName}<{string.Join(",", t.GenericTypeArguments.Select(ta => ComputeTypeKey(ta)))}>";
            }

            return t.FullName;
        }

        public void AppendEventTypeMatcher(Type eventType, LambdaEventMatcher matcher)
        {
            var typeKey = ComputeTypeKey(eventType);
            if (_eventTypeMatchers.ContainsKey(typeKey))
            {
                throw new InvalidOperationException(
                    "matcher for event type has already been registered: " + typeKey);
            }
            _logger.LogInformation($"Appending Matcher for Event Type key [{typeKey}]");
            _eventTypes.Add(typeKey);
            _eventTypeMatchers.Add(typeKey, matcher);
        }

        public void InsertEventTypeMatcher(Type eventType, LambdaEventMatcher matcher)
        {
            var typeKey = ComputeTypeKey(eventType);
            if (_eventTypeMatchers.ContainsKey(typeKey))
            {
                throw new InvalidOperationException(
                    "matcher for event type has already been registered: " + typeKey);
            }
            _logger.LogInformation($"Prepending Matcher for Event Type key [{typeKey}]");
            _eventTypes.Insert(0, typeKey);
            _eventTypeMatchers.Add(typeKey, matcher);
        }

        public void SetHandler(Type eventType, LambdaEventHandler handler)
        {
            var typeKey = ComputeTypeKey(eventType);
            if (!_eventTypeMatchers.ContainsKey(typeKey))
            {
                throw new InvalidOperationException(
                    "matcher for event type has not been registered: " + typeKey);
            }
            if (_eventTypeHandlers.ContainsKey(typeKey))
            {
                throw new InvalidOperationException(
                    "handler for event type has already been registered: " + typeKey);
            }
            _logger.LogInformation($"Setting Handler for Event Type key [{typeKey}]");
            _eventTypeHandlers.Add(typeKey, handler);
        }

        public async Task<Stream> DecodeEventAsync(Stream eventDataStream,
                ILambdaContext context, object paramData = null)
        {
            _logger.LogInformation("Decoding event input...");

            using (eventDataStream)
            using (var textReader = new StreamReader(eventDataStream))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                var jtoken = JToken.ReadFrom(jsonReader);

                _logger.LogDebug("Parsed JSON payload: " + jtoken);
                foreach (var mh in _matcherHandlers)
                {
                    _logger.LogDebug($"Testing Custom Matcher [{mh.matcher.Method.Name}]");
                    if (mh.matcher(jtoken))
                    {
                        _logger.LogDebug("  FOUND MATCH!");
                        var result = await mh.handler(jtoken, context, paramData);
                        return EncodeResult(result);
                    }
                }

                foreach (var eventType in _eventTypes)
                {
                    _logger.LogDebug($"Testing Match for Event Type [{eventType}]");
                    if (_eventTypeMatchers[eventType](jtoken))
                    {
                        _logger.LogDebug("  FOUND MATCH!");
                        var result = await _eventTypeHandlers[eventType](jtoken, context, paramData);
                        return EncodeResult(result);
                    }
                }

                if (_defaultHandler != null)
                {
                    _logger.LogDebug("Invoking DEFAULT");
                    var result = await _defaultHandler(jtoken, context, paramData);
                    return EncodeResult(result);
                }
            }

            throw new Exception("no matching handlers found and no default handler registered");
        }

        public Stream EncodeResult(TResult result)
        {
            var ms = new MemoryStream();
            _lambdaSer.Serialize(result, ms);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }

        // Based on this guidance:
        //    https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to#jsondocument-and-jsonelement-compared-to-jtoken-like-jobject-jarray
        //    https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to#how-to-search-a-jsondocument-and-jsonelement-for-sub-elements
        // The JsonDocument from System.Text.Json is *NOT* optimized for searches and lookups
        // therefore we base our serializer on the Newtonsoft serializer because that's exactly
        // what we want to be able to do for multi-event handling, where we're doing inspection
        // of the JSON document in order to match on a particular event type.
        public class DecoderLambdaJsonSerializer : LambdaJsonSerializer
        {
            private static JsonSerializerSettings _settings;

            public DecoderLambdaJsonSerializer() : base(SaveSettings)
            { }

            public JsonSerializerSettings DefaultSettings => _settings;

            private static void SaveSettings(JsonSerializerSettings settings)
            {
                _settings = settings;
            }
        }

        // public class DecoderLambdaJsonSerializer : DefaultLambdaJsonSerializer
        // {
        //     public JsonSerializerOptions GetOptions() => base.SerializerOptions;
        // }

/*
        // We use this string to string map so that we don't need
        // have a compile-time dependency to each event class that
        // we support and instead use reflection to match the event
        private static readonly IDictionary<string, string> _eventToJsonPathMap =
            new Dictionary<string, string>
            {
                // This was assembled with the help of:
                //    https://docs.aws.amazon.com/lambda/latest/dg/eventsources.html

                ["S3Event"] = "Records[0].s3",
                ["SNSEvent"] = "Records[0].Sns",
                ["CloudWatchLogsEvent"] = "awslogs",

                // This one is WRONG in the link above, but is clarified here:
                //    https://docs.aws.amazon.com/ses/latest/DeveloperGuide/receiving-email-action-lambda.html
                ["SESEvent"] = "Records[0].ses",
            };

        

        public async Task<Stream> DecodeEventAsync(Stream eventDataStream,
                ILambdaContext context, object paramData = null)
        {
            using (eventDataStream)
            using (var jdoc = await JsonDocument.ParseAsync(eventDataStream))
            {
                jdoc.RootElement

                eventData = await sr.ReadToEndAsync();
            }

            return await DecodeEventAsync(eventData, context, paramData);
        }

        public async Task<Stream> DecodeEventAsync(JsonDocument jdoc,
            ILambdaContext context, object paramData = null)
        {

            var json = JToken.Parse(eventData);
            if (json is JObject jobj)
            {
                foreach (var custom in _customEventsMap)
                {
                    if (custom.Matcher(jobj))
                    {
                        context.Logger.LogLine("Handling custom-matched event");
                        if (custom.LogEventData)
                        {
                            context.Logger.LogLine("...for event data:");
                            context.Logger.LogLine(eventData);
                        }
                        return EncodeResult(await custom.Handler(jobj, context, paramData));
                    }
                }

                foreach (var handledEvent in _eventHandlers)
                {
                    string jsonPath = handledEvent.Key;
                    if (json.SelectToken(jsonPath) != null)
                    {
                        context.Logger.LogLine($"Handling type-named event for [{jsonPath}]");
                        if (handledEvent.Value.LogEventData)
                        {
                            context.Logger.LogLine("...for event data:");
                            context.Logger.LogLine(eventData);
                        }

                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(eventData)))
                        {
                            var result = await handledEvent.Value.Handler(ms, context, paramData);
                            return EncodeResult(result);
                        }
                    }
                }
            }

            if (_defaultEventHandler != null)
            {
                context.Logger.LogLine($"Handling with DEFAULT event handler");
                if (_defaultEventHandlerLogEventData)
                {
                    context.Logger.LogLine("...for event data:");
                    context.Logger.LogLine(eventData);
                }
                return EncodeResult(await _defaultEventHandler(eventData, context, paramData));
            }
            
            throw new InvalidOperationException(
                    "No matching event handlers found and no default handler registered");
        }

        public Stream EncodeResult(TResult result)
        {
            var ms = new MemoryStream();
            _lambdaSer.Serialize(result, ms);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }


        private IList<CustomEventMapping<TResult>> _customEventsMap = new List<CustomEventMapping<TResult>>();

        private Func<string, ILambdaContext, object, Task<TResult>> _defaultEventHandler;
        private bool _defaultEventHandlerLogEventData;
        private IDictionary<string, TypeNamedEventMapping<TResult>> _eventHandlers =
                new Dictionary<string, TypeNamedEventMapping<TResult>>();

        private ILambdaSerializer _lambdaSer =
                new Amazon.Lambda.Serialization.Json.JsonSerializer();
        
        /// <summary>
        /// Used to handle custom event types based on a predicate matching function.
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="handler"></param>
        /// <typeparam name="TEvent"></typeparam>
        public void Handle<TEvent>(Func<JToken, bool> matcher, Func<TEvent, ILambdaContext, object,
            Task<TResult>> handler, bool logEventData = false)
        {
            _customEventsMap.Add(new CustomEventMapping<TResult>
            {
                TargetType = typeof(TEvent),
                Matcher = matcher,
                Handler = async (jt, ctx, obj) => {
                    var ev = jt.ToObject<TEvent>();
                    return await handler(ev, ctx, obj);
                },
                LogEventData = logEventData,
            });
        }

        /// <summary>
        ///  Used to handle default/built-in event types.
        /// </summary>
        /// <param name="handler"></param>
        /// <typeparam name="TEvent"></typeparam>
        public void Handle<TEvent>(Func<TEvent, ILambdaContext, object, Task<TResult>> handler,
            bool logEventData = false)
        {
            var eventType = typeof(TEvent).Name;

            if (!_eventToJsonPathMap.TryGetValue(eventType, out var jsonPath))
                throw new NotSupportedException($"Event type named [{eventType}] is not supported");

            if (_eventHandlers.ContainsKey(jsonPath))
                throw new ArgumentException("Event type is already registered for handling");

            Func<Stream, ILambdaContext, object, Task<TResult>> wrappedHandler = async (evDataStream, ctx, obj) => {
                var ev = _lambdaSer.Deserialize<TEvent>(evDataStream);
                return await handler((TEvent)ev, ctx, obj);
            };
            _eventHandlers[jsonPath] = new TypeNamedEventMapping<TResult>
            {
                Handler = wrappedHandler,
                LogEventData = logEventData,
            };
        }

        public void HandleDefault(Func<string, ILambdaContext, object, Task<TResult>> defaultHandler,
            bool logEventData = false)
        {
            _defaultEventHandler = defaultHandler;
            _defaultEventHandlerLogEventData = logEventData;
        }

        public async Task<Stream> DecodeEventAsync(Stream eventDataStream,
                ILambdaContext context, object paramData = null, bool logEventData = false)
        {
            string eventData;

            using (eventDataStream)
            using (var sr = new StreamReader(eventDataStream))
            {
                eventData = await sr.ReadToEndAsync();
            }

            if (logEventData)
            {
                context.Logger.LogLine("Got Event Data:");
                context.Logger.LogLine(eventData);
            }

            return await DecodeEventAsync(eventData, context, paramData);
        }

        public async Task<Stream> DecodeEventAsync(string eventData,
            ILambdaContext context, object paramData = null)
        {

            var json = JToken.Parse(eventData);
            if (json is JObject jobj)
            {
                foreach (var custom in _customEventsMap)
                {
                    if (custom.Matcher(jobj))
                    {
                        context.Logger.LogLine("Handling custom-matched event");
                        if (custom.LogEventData)
                        {
                            context.Logger.LogLine("...for event data:");
                            context.Logger.LogLine(eventData);
                        }
                        return EncodeResult(await custom.Handler(jobj, context, paramData));
                    }
                }

                foreach (var handledEvent in _eventHandlers)
                {
                    string jsonPath = handledEvent.Key;
                    if (json.SelectToken(jsonPath) != null)
                    {
                        context.Logger.LogLine($"Handling type-named event for [{jsonPath}]");
                        if (handledEvent.Value.LogEventData)
                        {
                            context.Logger.LogLine("...for event data:");
                            context.Logger.LogLine(eventData);
                        }

                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(eventData)))
                        {
                            var result = await handledEvent.Value.Handler(ms, context, paramData);
                            return EncodeResult(result);
                        }
                    }
                }
            }

            if (_defaultEventHandler != null)
            {
                context.Logger.LogLine($"Handling with DEFAULT event handler");
                if (_defaultEventHandlerLogEventData)
                {
                    context.Logger.LogLine("...for event data:");
                    context.Logger.LogLine(eventData);
                }
                return EncodeResult(await _defaultEventHandler(eventData, context, paramData));
            }
            
            throw new InvalidOperationException(
                    "No matching event handlers found and no default handler registered");
        }

        public Stream EncodeResult(TResult result)
        {
            var ms = new MemoryStream();
            _lambdaSer.Serialize(result, ms);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }
*/
    }

/*
    internal class TypeNamedEventMapping<TResult>
    {
        public Func<Stream, ILambdaContext, object, Task<TResult>> Handler { get; set; }
        public bool LogEventData { get; set; }
    }

    internal class CustomEventMapping<TResult>
    {
        public Type TargetType { get; set; }
        public Func<JToken, bool> Matcher { get; set; }
        public Func<JToken, ILambdaContext, object, Task<TResult>> Handler { get; set; }
        public bool LogEventData { get; set; }
    }
*/
}