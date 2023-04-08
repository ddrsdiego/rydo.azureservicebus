namespace Rydo.AzureServiceBus.Client.Topics
{
    using System;

    internal static class Topic
    {
        public static string Sanitize(string topicName) => topicName.ToLowerInvariant();
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TopicProducerAttribute : Attribute
    {
        internal const int TopicNamePosition = 0;
        internal const string FullNameTopicProducerAttribute =
            "Rydo.AzureServiceBus.Client.Topics.TopicProducerAttribute";

        public TopicProducerAttribute(string topicName)
        {
            TopicName = topicName;
        }

        public string TopicName { get; set; }
    }
}