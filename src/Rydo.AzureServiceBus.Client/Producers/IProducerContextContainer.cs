namespace Rydo.AzureServiceBus.Client.Producers
{
    using System;
    using System.Collections.Immutable;

    public interface IProducerContextContainer
    {
        void AddProducers(string topicName);

        void AddProducers(string topicName, Action<IProducerConfigurator> configurator);

        ImmutableDictionary<string, ProducerContext> Entries { get; }
    }
}