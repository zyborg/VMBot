using System;
using Microsoft.Extensions.DependencyInjection;

namespace Zyborg.AWS.Lambda
{
    public static class LambdaFunctionServicesExtensions
    {
        /// <summary>
        /// Allows adding logic preceding the invocation of
        /// <see cref="LambdaFunction{TFunction}.PrepareFinal(Microsoft.Extensions.Logging.ILogger{TFunction}, Microsoft.Extensions.Configuration.IConfiguration, IServiceProvider)" />.
        /// Multiple action instances can be registered and they will be invoked
        /// in the order registered before the <c>Configure</c> method is invoked.
        /// </summary>
        public static IServiceCollection AddPrePrepareFinalAction(
            this IServiceCollection services, Action<IServiceProvider> action)
        {
            services.AddScoped(sp => new LambdaFunctionPrePrepareFinalAction(action));
            return services;
        }

        /// <summary>
        /// Allows adding logic following the invocation of
        /// <see cref="LambdaFunction{TFunction}.PrepareFinal(Microsoft.Extensions.Logging.ILogger{TFunction}, Microsoft.Extensions.Configuration.IConfiguration, IServiceProvider)" />.
        /// Multiple action instances can be registered and they will be invoked
        /// in the order registered after the <c>Configure</c> method is invoked.
        /// </summary>
        public static IServiceCollection AddPostPrepareFinalAction(
            this IServiceCollection services, Action<IServiceProvider> action)
        {
            services.AddScoped(sp => new LambdaFunctionPostPrepareFinalAction(action));
            return services;
        }
    }
}