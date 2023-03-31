namespace Rydo.AzureServiceBus.Client.Configurations
{
    using Microsoft.Extensions.DependencyInjection;
    using Producers;
    using Receivers;
    using Subscribers;

    public sealed class AzureServiceBusClientConfigurator : IAzureServiceBusClientConfigurator
    {
        public AzureServiceBusClientConfigurator(IServiceCollection services)
        {
            Producers = new AzureServiceBusProducersConfigurator(services);
            Subscribers = new AzureServiceBusSubscribersConfigurator(services);
            Receiver = new AzureServiceBusReceiverConfigurator(services);
        }

        public IAzureServiceBusReceiverConfigurator Receiver { get; }
        public IAzureServiceBusProducersConfigurator Producers { get; }
        public IAzureServiceBusSubscribersConfigurator Subscribers { get; }
    }
}