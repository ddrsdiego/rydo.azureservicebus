namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    public interface ISubscriberContextContainer
    {
        void WithTypes(IEnumerable<Type> types);

        ImmutableDictionary<string, SubscriberContext> Contexts { get; }

        void AddSubscriber(string topicName);

        void AddSubscriber(string topicName, Action<SubscriberConfiguratorBuilder> configurator);

        void AddSubscriber(string topicName, string subscriptionName);

        void AddSubscriber(string topicName, string subscriptionName,
            Action<SubscriberConfiguratorBuilder> configurator);

        bool TryGetConsumerContext(string topicName, out SubscriberContext context);
    }

    public interface IConsumerConfigurator
    {
        string TopicName { get; }

        bool NeverAutoDelete { get; set; }

        int AutoDeleteAfterIdleInHours { get; set; }

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
        public bool NeverAutoDelete { get; set; }
        public int MaxMessages { get; set; }
        public int BufferSize { get; set; }
        public string SubscriptionName { get; }
        public int LockDurationInMinutes { get; set; }
        public int MaxDeliveryCount { get; set; }
        public int AutoDeleteAfterIdleInHours { get; set; }
    }
}