namespace Rydo.AzureServiceBus.Client.Producers
{
    using System;
    using Headers;
    using Topics;

    public readonly struct ProducerRequest
    {
        private ProducerRequest(string topicName, string partitionKey, object message, MessageHeaders headers)
        {
            PartitionKey = partitionKey ?? throw new ArgumentNullException(nameof(partitionKey));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            TopicName = Topic.Sanitize(topicName) ?? throw new ArgumentNullException(nameof(topicName));
            MessageHeaders = headers;
            RequestedAt = DateTime.UtcNow;
        }

        public readonly string TopicName;
        public readonly string PartitionKey;
        public readonly object Message;
        public readonly DateTime RequestedAt;
        internal readonly MessageHeaders MessageHeaders;

        internal static ProducerRequest GetInstance(string topicName, object message, MessageHeaders headers) =>
            GetInstance(topicName, Guid.NewGuid().ToString(), message, headers);

        internal static ProducerRequest GetInstance(string topicName, string partitionKey, object message,
            MessageHeaders headers) => new ProducerRequest(topicName, partitionKey, message,headers);
    }
}