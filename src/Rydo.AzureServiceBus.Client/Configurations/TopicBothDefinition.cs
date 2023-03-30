namespace Rydo.AzureServiceBus.Client.Configurations
{
    internal sealed class TopicBothDefinition : TopicDefinition
    {
        public TopicBothDefinition(string topicName, string subscriptionName, int lockDurationInMinutes = 60,
            int maxDeliveryCount = 10) 
            : base(TopicDirections.Both, topicName, subscriptionName, lockDurationInMinutes, maxDeliveryCount)
        {
        }
    }
}