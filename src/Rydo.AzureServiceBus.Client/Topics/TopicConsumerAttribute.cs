namespace Rydo.AzureServiceBus.Client.Topics
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TopicConsumerAttribute : TopicAttribute
    {
        public const string FullNameTopicConsumerAttribute =
            "Rydo.AzureServiceBus.Client.Topics.TopicConsumerAttribute";

        public TopicConsumerAttribute(string topicName)
            : base(topicName)
        {
        }

        public TopicConsumerAttribute(string topicName, string subscriptionName)
            : base(topicName)
        {
            SubscriptionName = subscriptionName;
        }
        
        public string SubscriptionName { get; }
    }
}