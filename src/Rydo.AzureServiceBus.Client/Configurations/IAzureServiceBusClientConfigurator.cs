namespace Rydo.AzureServiceBus.Client.Configurations
{
    using Producers;
    using Receivers;

    public interface IAzureServiceBusClientConfigurator
    {
        IAzureServiceBusReceiverConfigurator Receiver { get; }
        IAzureServiceBusProducersConfigurator Producers { get; }
        // IAzureServiceBusSubscribersConfigurator Subscribers { get; }
    }
}