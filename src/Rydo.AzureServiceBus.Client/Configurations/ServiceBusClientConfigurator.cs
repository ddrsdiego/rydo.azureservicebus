namespace Rydo.AzureServiceBus.Client.Configurations
{
    using Host;
    using Microsoft.Extensions.DependencyInjection;
    using Producers;
    using Receivers;

    public sealed class ServiceBusClientConfigurator : IServiceBusClientConfigurator
    {
        internal ServiceBusClientConfigurator(IServiceCollection services)
        {
            Collection = services;
            Host = new ServiceBusHostConfigurator(Collection);
            Receiver = new ServiceBusReceiverConfigurator(Collection);
            Producers = new ServiceBusProducersConfigurator(Collection);
        }

        public IServiceCollection Collection { get; }
        public ServiceBusHostConfigurator Host { get; }
        public ServiceBusReceiverConfigurator Receiver { get; }
        public ServiceBusProducersConfigurator Producers { get; }
    }
}