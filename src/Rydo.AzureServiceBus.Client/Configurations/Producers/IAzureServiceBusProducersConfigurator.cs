namespace Rydo.AzureServiceBus.Client.Configurations.Producers
{
    using System;
    using Rydo.AzureServiceBus.Client.Producers;

    public interface IAzureServiceBusProducersConfigurator
    {
        void Configure(Action<IProducerContextContainer> container);
    }
}