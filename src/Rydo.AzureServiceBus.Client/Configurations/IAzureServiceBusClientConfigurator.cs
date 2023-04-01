namespace Rydo.AzureServiceBus.Client.Configurations
{
    using Producers;
    using Receivers;
    using Subscribers;

    public interface IAzureServiceBusClientConfigurator
    {
        IAzureServiceBusReceiverConfigurator Receiver { get; }
        IAzureServiceBusProducersConfigurator Producers { get; }
        IAzureServiceBusSubscribersConfigurator Subscribers { get; }
    }
}