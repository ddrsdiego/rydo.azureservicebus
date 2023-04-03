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
            Services = services;
            Host = new ServiceBusHostConfigurator(Services);
            Receiver = new ServiceBusReceiverConfigurator(Services);
            Producers = new ServiceBusProducersConfigurator(Services);
        }

        public IServiceCollection Services { get; }
        public ServiceBusHostConfigurator Host { get; }
        public ServiceBusReceiverConfigurator Receiver { get; }
        public ServiceBusProducersConfigurator Producers { get; }
    }
}