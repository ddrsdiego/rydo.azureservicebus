namespace Rydo.AzureServiceBus.Client.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    public interface IConsumerContextContainer
    {
        void WithTypes(IEnumerable<Type> types);

        ImmutableDictionary<string, ConsumerContext> Contexts { get; }

        void AddSubscriber(string topicName);

        void AddSubscriber(string topicName, Action<ConsumerConfiguratorBuilder> configurator);

        bool TryGetConsumerContext(string topicName, out ConsumerContext context);
    }

    public interface IConsumerConfigurator
    {
        string TopicName { get; }

        int MaxMessages { get; set; }

        int BufferSize { get; set; }

        string SubscriptionName { get; }

        int LockDurationInMinutes { get; set; }

        int MaxDeliveryCount { get; set; }
    }

    internal sealed class ConsumerConfigurator : IConsumerConfigurator
    {
        public ConsumerConfigurator(string topicName, string subscriptionName)
        {
            TopicName = topicName;
            SubscriptionName = subscriptionName;
        }

        public string TopicName { get; }
        public int MaxMessages { get; set; }
        public int BufferSize { get; set; }
        public string SubscriptionName { get; }
        public int LockDurationInMinutes { get; set; }
        public int MaxDeliveryCount { get; set; }
    }
}