namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using Azure.Messaging.ServiceBus;

    public sealed class SubscriberSpecification
    {
        internal SubscriberSpecification(IConsumerConfigurator consumer)
        {
            Consumer = consumer;
            TopicName = Consumer.TopicName;
            SubscriptionName = Consumer.SubscriptionName;
            PrefetchCount = Consumer.PrefetchCount;
            MaxMessages = Consumer.MaxMessages;
            MaxDeliveryCount = Consumer.MaxDeliveryCount;
            ReceiveMode = consumer.ReceiveMode;
            AutoDeleteOnIdle = TimeSpan.FromHours(consumer.AutoDeleteAfterIdleInHours);
            LockDurationInSeconds = TimeSpan.FromSeconds(Consumer.LockDurationInSeconds);
        }

        public readonly string TopicName;
        public readonly string SubscriptionName;
        public readonly int MaxMessages;
        public readonly TimeSpan LockDurationInSeconds;
        public readonly int MaxDeliveryCount;
        internal readonly IConsumerConfigurator Consumer;
        public readonly ServiceBusReceiveMode ReceiveMode;
        public readonly int PrefetchCount;
        public readonly TimeSpan AutoDeleteOnIdle;
    }
}