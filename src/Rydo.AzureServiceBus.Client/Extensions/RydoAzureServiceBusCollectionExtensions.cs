namespace Rydo.AzureServiceBus.Client.Extensions
{
    using System;
    using Configurations;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Middlewares.Consumers;

    public static class RydoAzureServiceBusCollectionExtensions
    {
        public static void AddAzureServiceBusClient(this IServiceCollection services,
            Action<IAzureServiceBusClientConfigurator> clientConfigurator)
        {
            var configurator = new AzureServiceBusClientConfigurator(services);
            clientConfigurator(configurator);

            services.TryAddSingleton<IMessageRecordFactory, MessageRecordFactory>();

            // services.AddSingleton(sp => new ServiceBusAdministrationClient(sbConnectionString));
            // services.AddAzureClients(config => { config.AddServiceBusClient(sbConnectionString); });
        }
    }
}