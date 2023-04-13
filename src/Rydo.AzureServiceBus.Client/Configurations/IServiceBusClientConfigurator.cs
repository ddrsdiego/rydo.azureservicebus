namespace Rydo.AzureServiceBus.Client.Configurations
{
    using Host;
    using Microsoft.Extensions.DependencyInjection;
    using Producers;
    using Receivers;

    public interface IServiceBusClientConfigurator
    {
        IServiceCollection Collection { get; }
        ServiceBusHostConfigurator Host { get; }
        ServiceBusReceiverConfigurator Receiver { get; }
        ServiceBusProducersConfigurator Producers { get; }
    }
}