namespace Rydo.AzureServiceBus.Client.Configurations
{
    using System;
    using Topics;

    public abstract class TopicDefinition
    {
        protected TopicDefinition(string direction, string topicName, string subscriptionName,
            int lockDurationInMinutes = TopicConsumerDefaultValues.LockDurationInSeconds, int maxDeliveryCount = TopicConsumerDefaultValues.MaxDeliveryCount)
        {
            Direction = direction;
            TopicName = Topic.Sanitize(topicName);
            SubscriptionName = subscriptionName;
            MaxDeliveryCount = maxDeliveryCount;
            LockDurationInSeconds = TimeSpan.FromSeconds(lockDurationInMinutes);
        }

        public string Direction { get; }
        public string TopicName { get; }
        public string SubscriptionName { get; }
        public int MaxDeliveryCount { get; }
        public TimeSpan LockDurationInSeconds { get; }
    }
}