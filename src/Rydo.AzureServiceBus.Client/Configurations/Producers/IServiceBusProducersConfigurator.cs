namespace Rydo.AzureServiceBus.Client.Configurations.Producers
{
    using System;
    using Rydo.AzureServiceBus.Client.Producers;

    public interface IServiceBusProducersConfigurator
    {
        void Configure(Action<IProducerContextContainer> container);
    }
}