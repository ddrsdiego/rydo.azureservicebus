namespace Rydo.AzureServiceBus.Client.Configurations
{
    using Host;
    using Microsoft.Extensions.DependencyInjection;
    using Producers;
    using Receivers;

    public interface IServiceBusClientConfigurator
    {
        IServiceCollection Services { get; }
        ServiceBusHostConfigurator Host { get; }
        ServiceBusReceiverConfigurator Receiver { get; }
        ServiceBusProducersConfigurator Producers { get; }
    }
}