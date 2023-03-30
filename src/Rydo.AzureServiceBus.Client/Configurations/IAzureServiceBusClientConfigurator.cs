namespace Rydo.AzureServiceBus.Client.Configurations
{
    using Producers;
    using Queues;
    using Subscribers;

    public interface IAzureServiceBusClientConfigurator
    {
        IAzureServiceBusProducersConfigurator Producers { get; }
        IAzureServiceBusQueuesConfigurator Queues { get; }
        IAzureServiceBusSubscribersConfigurator Subscribers { get; }
    }
}