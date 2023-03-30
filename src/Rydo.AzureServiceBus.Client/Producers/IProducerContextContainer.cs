namespace Rydo.AzureServiceBus.Client.Producers
{
    using System;
    using System.Collections.Immutable;

    public interface IProducerContextContainer
    {
        void TryAdd(string topicName);

        void TryAdd(string topicName, Action<IProducerConfigurator> configurator);

        ImmutableDictionary<string, ProducerContext> Entries { get; }
    }
}