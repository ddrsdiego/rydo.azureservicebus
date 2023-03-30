namespace Rydo.AzureServiceBus.Client.Configurations
{
    using System;
    using Producers;

    public interface IAzureServiceBusProducerConfigurator
    {
        void Configure(Action<IProducerContextContainer> container);
    }
}