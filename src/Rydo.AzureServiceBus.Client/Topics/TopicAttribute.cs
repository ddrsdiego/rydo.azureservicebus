namespace Rydo.AzureServiceBus.Client.Topics
{
    using System;

    public abstract class TopicAttribute : Attribute
    {
        internal const int TopicNamePosition = 0;

        protected TopicAttribute(string topicName)
        {
            TopicName = Topic.Sanitize(topicName);
        }
        
        public string TopicName { get; }
    }
}