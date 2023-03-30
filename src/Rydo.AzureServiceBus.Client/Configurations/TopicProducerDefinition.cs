namespace Rydo.AzureServiceBus.Client.Configurations
{
    internal sealed class TopicProducerDefinition : TopicDefinition
    {
        public TopicProducerDefinition(string topicName)
            : base(TopicDirections.Producer, topicName, string.Empty)
        {
        }
    }
}