namespace Rydo.AzureServiceBus.Client.Configurations
{
    using Microsoft.Extensions.DependencyInjection;
    using Producers;
    using Queues;
    using Subscribers;

    public sealed class AzureServiceBusClientConfigurator : IAzureServiceBusClientConfigurator
    {
        public AzureServiceBusClientConfigurator(IServiceCollection services)
        {
            Producers = new AzureServiceBusProducersConfigurator(services);
            Queues = new AzureServiceBusQueuesConfigurator(services);
            Subscribers = new AzureServiceBusSubscribersConfigurator(services);
        }

        public IAzureServiceBusProducersConfigurator Producers { get; }
        public IAzureServiceBusQueuesConfigurator Queues { get; }
        public IAzureServiceBusSubscribersConfigurator Subscribers { get; }
    }
}