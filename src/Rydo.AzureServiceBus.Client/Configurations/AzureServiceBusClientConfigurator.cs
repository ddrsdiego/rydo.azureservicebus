namespace Rydo.AzureServiceBus.Client.Configurations
{
    using Microsoft.Extensions.DependencyInjection;
    using Producers;
    using Receivers;

    public sealed class AzureServiceBusClientConfigurator : IAzureServiceBusClientConfigurator
    {
        public AzureServiceBusClientConfigurator(IServiceCollection services)
        {
            Producers = new AzureServiceBusProducersConfigurator(services);
            Receiver = new AzureServiceBusReceiverConfigurator(services);
        }

        public IAzureServiceBusReceiverConfigurator Receiver { get; }
        public IAzureServiceBusProducersConfigurator Producers { get; }
    }
}