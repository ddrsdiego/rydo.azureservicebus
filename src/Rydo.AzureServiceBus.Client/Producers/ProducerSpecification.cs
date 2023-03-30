namespace Rydo.AzureServiceBus.Client.Producers
{
    public sealed class ProducerSpecification
    {
        public readonly string TopicName;

        public ProducerSpecification(string topicName)
        {
            TopicName = topicName;
        }
    }
}