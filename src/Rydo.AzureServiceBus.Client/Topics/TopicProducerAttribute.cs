namespace Rydo.AzureServiceBus.Client.Topics
{
    internal static class Topic
    {
        public static string Sanitize(string topicName) => topicName.ToLowerInvariant();
    }

    public sealed class TopicProducerAttribute : TopicAttribute
    {
        public const string FullNameTopicProducerAttribute =
            "Rydo.AzureServiceBus.Client.Topics.TopicProducerAttribute";

        public TopicProducerAttribute(string topicName)
            : base(topicName)
        {
        }
    }
}