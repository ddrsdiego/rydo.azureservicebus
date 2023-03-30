namespace Rydo.AzureServiceBus.Client.Producers
{
    public interface ITopicProducerManager
    {
        bool TryExtractTopicName(object model, out string topicName);
    }
}