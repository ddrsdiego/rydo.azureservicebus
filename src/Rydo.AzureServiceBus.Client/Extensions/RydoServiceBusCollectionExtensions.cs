namespace Rydo.AzureServiceBus.Client.Extensions
{
    using System;
    using Configurations;
    using Configurations.Extensions;
    using Microsoft.Extensions.DependencyInjection;

    public static class RydoServiceBusCollectionExtensions
    {
        public static void AddAzureServiceBusClient(this IServiceCollection services,
            Action<IServiceBusClientConfigurator> clientConfigurator)
        {
            var configurator = new ServiceBusClientConfigurator(services);

            clientConfigurator(configurator);
            configurator.Configure();
        }
    }
}