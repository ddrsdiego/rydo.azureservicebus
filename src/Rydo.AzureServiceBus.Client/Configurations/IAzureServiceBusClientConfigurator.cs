namespace Rydo.AzureServiceBus.Client.Configurations
{
    public interface IAzureServiceBusClientConfigurator
    {
        IAzureServiceBusProducerConfigurator Producer { get; }

        IAzureServiceBusConsumerConfigurator Consumer { get; }
    }
}