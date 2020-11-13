using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Zyborg.AWS.Lambda
{
    internal delegate void LambdaFunctionPrePrepareFinalAction(IServiceProvider services);
    internal delegate void LambdaFunctionPostPrepareFinalAction(IServiceProvider services);

    /// <summary>
    /// A base class to structure AWS Lambda Functions in a .NET-ish approach.
    /// This class provides a structure that is heavily inspired by the .NET Core approach of
    /// structuring an application in the spirit of the Host builder model.  However, due
    /// to the differences in lifecycle and invocation model, there are differences to better
    /// suit the Functions invocation model.
    /// </summary>
    /// <remarks>
    /// A subclass should invoke the <see cref="Initialize"/> method within its
    /// constructor to complete the initialization and preparation of the hosting
    /// environment, including components of logging, configuration and service
    /// registration.
    /// <p>
    /// Subclasses can also override any of the <c>Prepare*</c> methods to customize the
    /// behavior of that stage of the lifecycle of a Lambda Function.  If they wish to
    /// simply inherit and add to the default behavior of that stage, the should simply
    /// invoke the base implementation.
    /// </remarks>
    /// <typeparam name="TFunction"></typeparam>
    public class LambdaFunction<TFunction> where TFunction : LambdaFunction<TFunction>
    {
        private IConfigurationRoot _config;
        private IServiceProvider _services;
        private ILogger _logger;
        private ILogger<TFunction> _subclassLogger;

        protected LambdaFunction()
        { }

        /// <summary>
        /// This should be invoked during a subclass' constructor to initiate and
        /// complete the initialization and preparation of the hosting environment.
        /// </summary>
        protected void Initialize()
        {
            SetupAppFeatures();
        }

        /// <summary>
        /// Customize app configuration resolution.
        /// Be sure to invoke the base implementation if you
        /// want to preserve the default base class behavior.
        /// </summary>
        protected virtual void PrepareAppConfiguration(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();
        }

        /// <summary>
        /// Customize app logging services.
        /// Be sure to invoke the base implementation if you
        /// want to preserve the default base class behavior.
        /// </summary>
        protected virtual void PrepareAppLogging(ILoggingBuilder builder, IConfiguration config)
        {
            var opts = new LambdaLoggerOptions(config);
            builder.AddLambdaLogger(opts);
        }

        /// <summary>
        /// Customize dependency injection services.
        /// Be sure to invoke the base implementation if you
        /// want to preserve the default base class behavior.
        /// </summary>
        protected virtual void PrepareServices(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IConfiguration>(config);
            services.AddLogging(logging => PrepareAppLogging(logging, config));
        }

        protected virtual void PrepareFinal(ILogger<TFunction> logger, IConfiguration config, IServiceProvider services)
        { }

        private void SetupAppFeatures()
        {
            _config = SetupConfiguration();

            // DEBUG:
            // var configMap = _config.AsEnumerable();
            // Console.WriteLine("BUILT CONFIG:");
            // Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(configMap));
            // Console.WriteLine("ENV:");
            // Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(Environment.GetEnvironmentVariables()));

            _services = SetupServices();

            _logger = _services.GetRequiredService<ILogger<LambdaFunction<TFunction>>>();
            _subclassLogger = _services.GetRequiredService<ILogger<TFunction>>();

            var prePrepareFinalActions = _services.GetServices<LambdaFunctionPrePrepareFinalAction>();
            var postPrepareFinalActions = _services.GetServices<LambdaFunctionPostPrepareFinalAction>();

            foreach (var action in prePrepareFinalActions)
            {
                action(_services);
            }

            PrepareFinal(_subclassLogger, _config, _services);

            foreach (var action in postPrepareFinalActions)
            {
                action(_services);
            }
        }

        private IConfigurationRoot SetupConfiguration()
        {
            var builder = new ConfigurationBuilder();

            PrepareAppConfiguration(builder);

            return builder.Build();
        }

        private IServiceProvider SetupServices()
        {
            var services = new ServiceCollection();

            PrepareServices(services, _config);

            return services.BuildServiceProvider();
        }
    }
}