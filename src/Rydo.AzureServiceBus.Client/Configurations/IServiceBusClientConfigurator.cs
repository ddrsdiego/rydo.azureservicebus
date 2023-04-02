namespace Rydo.AzureServiceBus.Client.Configurations
{
    using Host;
    using Producers;
    using Receivers;

    public interface IServiceBusClientConfigurator
    {
        ServiceBusHostConfigurator Host { get; }
        ServiceBusReceiverConfigurator Receiver { get; }
        ServiceBusProducersConfigurator Producers { get; }
    }
}