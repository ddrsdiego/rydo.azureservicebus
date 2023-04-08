namespace Rydo.AzureServiceBus.Client.Topics
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TopicConsumerAttribute : Attribute
    {
        internal const int TopicNamePosition = 1;
        internal const int ContractTypeNamePosition = 0;
        
        internal const string FullNameTopicConsumerAttribute =
            "Rydo.AzureServiceBus.Client.Topics.TopicConsumerAttribute";

        public TopicConsumerAttribute(Type contractType, string topicName)
        {
            TopicName = topicName ?? throw new ArgumentNullException(nameof(topicName));
            ContractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        }
        
        public Type ContractType { get; }
        public string TopicName { get; }
    }
}