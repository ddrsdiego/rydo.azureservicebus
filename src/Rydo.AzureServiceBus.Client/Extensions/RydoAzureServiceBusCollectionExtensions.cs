namespace Rydo.AzureServiceBus.Client.Extensions
{
    using System;
    using Configurations;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Middlewares.Consumers;
    using Middlewares.Extensions;
    using Services;

    public static class RydoAzureServiceBusCollectionExtensions
    {
        public static void AddAzureServiceBusClient(this IServiceCollection services,
            Action<IAzureServiceBusClientConfigurator> clientConfigurator)
        {
            var configurator = new AzureServiceBusClientConfigurator(services);
            clientConfigurator(configurator);

            services.AddMiddlewares();
            services.TryAddSingleton<IMessageRecordFactory, MessageRecordFactory>();
            services.AddHostedService<AzureServiceBusIntegrationHostedService>();
            
            // services.AddSingleton(sp => new ServiceBusAdministrationClient(sbConnectionString));
            // services.AddAzureClients(config => { config.AddServiceBusClient(sbConnectionString); });
            
        }
    }
}