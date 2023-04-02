namespace Rydo.AzureServiceBus.Client.Middlewares.Extensions
{
    using System.Collections.Generic;
    using Consumers;
    using Microsoft.Extensions.DependencyInjection;

    internal static class MiddlewareExtension
    {
        public static void AddMiddlewares(this IServiceCollection services)
        {
            var configs = new List<MiddlewareConfiguration>();

            configs.Insert(configs.Count,
                new MiddlewareConfiguration(typeof(DeserializerConsumerMiddleware), ServiceLifetime.Scoped));

            configs.Insert(configs.Count,
                new MiddlewareConfiguration(typeof(CustomConsumerMiddleware), ServiceLifetime.Scoped));
            
            configs.Insert(configs.Count,
                new MiddlewareConfiguration(typeof(DeadLetterHandleMiddleware), ServiceLifetime.Scoped));
            
            var container = new MiddlewareConfigurationContainer(configs,
                new MiddlewareConfiguration(typeof(CompleteMessageMiddleware), ServiceLifetime.Scoped));

            foreach (var middlewareConfiguration in container.Configs)
            {
                services.Add(new ServiceDescriptor(middlewareConfiguration.Type, middlewareConfiguration.Type,
                    middlewareConfiguration.Lifetime));
            }

            services.Add(new ServiceDescriptor(container.FinallyProcess.Type, container.FinallyProcess.Type,
                container.FinallyProcess.Lifetime));
            
            services.AddSingleton(configs);
            services.AddSingleton<IMiddlewareConfigurationContainer>(container);
        }
    }
}