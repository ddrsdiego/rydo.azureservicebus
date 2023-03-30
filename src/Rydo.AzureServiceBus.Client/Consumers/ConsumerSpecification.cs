namespace Rydo.AzureServiceBus.Client.Consumers
{
    public sealed class ConsumerSpecification
    {
        internal ConsumerSpecification(string topicName, string subscriptionName, int maxDelivery,
            int lockDurationInMinutes, int maxDeliveryCount)
        {
            TopicName = topicName;
            SubscriptionName = subscriptionName;
            MaxDelivery = maxDelivery;
            LockDurationInMinutes = lockDurationInMinutes;
            MaxDeliveryCount = maxDeliveryCount;
        }
        
        public readonly string TopicName;
        public readonly string SubscriptionName;
        public readonly int MaxDelivery;
        public readonly int LockDurationInMinutes;
        public readonly int MaxDeliveryCount;
    }
}