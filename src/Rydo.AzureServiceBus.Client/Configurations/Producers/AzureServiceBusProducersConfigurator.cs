namespace Rydo.AzureServiceBus.Client.Configurations.Producers
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Rydo.AzureServiceBus.Client.Producers;

    internal sealed class AzureServiceBusProducersConfigurator : IAzureServiceBusProducersConfigurator
    {
        private readonly IServiceCollection _services;
        private readonly IProducerContextContainer _producerContextContainer;

        public AzureServiceBusProducersConfigurator(IServiceCollection services)
        {
            _services = services;
            _producerContextContainer = new ProducerContextContainer(_services);
        }

        public void Configure(Action<IProducerContextContainer> container)
        {
            container(_producerContextContainer);
            _services.AddSingleton(_producerContextContainer);
        }
    }
}