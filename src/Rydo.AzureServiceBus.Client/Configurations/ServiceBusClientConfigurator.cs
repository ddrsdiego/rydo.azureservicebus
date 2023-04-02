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
            Host = new ServiceBusHostConfigurator(services);
            Receiver = new ServiceBusReceiverConfigurator(services);
            Producers = new ServiceBusProducersConfigurator(services);
        }

        public ServiceBusHostConfigurator Host { get; }
        public ServiceBusReceiverConfigurator Receiver { get; }
        public ServiceBusProducersConfigurator Producers { get; }
    }
}