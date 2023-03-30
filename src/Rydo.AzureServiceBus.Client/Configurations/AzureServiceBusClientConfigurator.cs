namespace Rydo.AzureServiceBus.Client.Configurations
{
    using Microsoft.Extensions.DependencyInjection;

    public sealed class AzureServiceBusClientConfigurator : IAzureServiceBusClientConfigurator
    {
        public AzureServiceBusClientConfigurator(IServiceCollection services)
        {
            Producer = new AzureServiceBusProducerConfigurator(services);
            Consumer = new AzureServiceBusConsumerConfigurator(services);
        }

        public IAzureServiceBusProducerConfigurator Producer { get; }
        public IAzureServiceBusConsumerConfigurator Consumer { get; }
    }
}