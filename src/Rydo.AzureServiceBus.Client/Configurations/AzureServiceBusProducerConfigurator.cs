namespace Rydo.AzureServiceBus.Client.Configurations
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Producers;

    internal sealed class AzureServiceBusProducerConfigurator : IAzureServiceBusProducerConfigurator
    {
        private readonly IServiceCollection _services;
        private readonly IProducerContextContainer _producerContextContainer;

        public AzureServiceBusProducerConfigurator(IServiceCollection services)
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