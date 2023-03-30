namespace Rydo.AzureServiceBus.Client.Producers
{
    using System;
    using System.Collections.Immutable;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Serialization;

    internal sealed class ProducerContextContainer : IProducerContextContainer
    {
        private readonly IServiceCollection _services;

        public ProducerContextContainer(IServiceCollection services)
        {
            _services = services;
            Entries = ImmutableDictionary<string, ProducerContext>.Empty;
        }

        public ImmutableDictionary<string, ProducerContext> Entries { get; private set; }

        public void AddProducers(string topicName)
        {
            AddProducers(topicName, configurator =>
            {
                configurator.Options = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
            });
        }

        public void AddProducers(string topicName, Action<IProducerConfigurator> configurator)
        {
            if (string.IsNullOrWhiteSpace(topicName)) throw new ArgumentNullException(nameof(topicName));

            if (Entries.TryGetValue(topicName, out var _))
                throw new InvalidOperationException(nameof(topicName));

            var producerConfigurator = new ProducerConfigurator();
            configurator(producerConfigurator);

            var producerSpecification = new ProducerSpecification(topicName);
            var producerContext =
                new ProducerContext(producerSpecification, null);
            
            _services.TryAddSingleton<ISerializer>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<SystemTextJsonSerializer>>();
                return new SystemTextJsonSerializer(logger, producerConfigurator.Options);
            });

            Entries = Entries.Add(producerContext.ProducerSpecification.TopicName, producerContext);
        }
    }
}