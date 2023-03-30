namespace Rydo.AzureServiceBus.Client.Configurations
{
    internal sealed class TopicConsumerDefinition : TopicDefinition
    {
        public TopicConsumerDefinition(string topicName, string subscriptionName, int lockDurationInMinutes = TopicConsumerDefaultValues.LockDurationInSeconds,
            int maxDeliveryCount = TopicConsumerDefaultValues.MaxDeliveryCount)
            : base(TopicDirections.Consumer, topicName, subscriptionName, lockDurationInMinutes, maxDeliveryCount)
        {
        }
    }
}