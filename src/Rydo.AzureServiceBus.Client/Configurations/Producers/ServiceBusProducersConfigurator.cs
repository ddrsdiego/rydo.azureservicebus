namespace Rydo.AzureServiceBus.Client.Configurations.Producers
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Rydo.AzureServiceBus.Client.Producers;

    public sealed class ServiceBusProducersConfigurator : IAzureServiceBusProducersConfigurator
    {
        private readonly IServiceCollection _services;
        private readonly IProducerContextContainer _producerContextContainer;

        internal ServiceBusProducersConfigurator(IServiceCollection services)
        {
            _services = services;
            _producerContextContainer = new ProducerContextContainer(_services);
        }

        public void Configure(Action<IProducerContextContainer> container)
        {
            container(_producerContextContainer);
            _services.TryAddSingleton(_producerContextContainer);
        }
    }
}