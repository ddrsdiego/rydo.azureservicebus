namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;

    public sealed class SubscriberSpecification
    {
        internal SubscriberSpecification(IConsumerConfigurator consumer)
        {
            Consumer = consumer;
            TopicName = Consumer.TopicName;
            SubscriptionName = Consumer.SubscriptionName;
            MaxMessages = Consumer.MaxMessages;
            MaxDeliveryCount = Consumer.MaxDeliveryCount;
            LockDurationInMinutes = TimeSpan.FromMinutes(Consumer.LockDurationInMinutes);
        }

        public readonly string TopicName;
        public readonly string SubscriptionName;
        public readonly int MaxMessages;
        public readonly TimeSpan LockDurationInMinutes;
        public readonly int MaxDeliveryCount;
        public readonly IConsumerConfigurator Consumer;
    }
}