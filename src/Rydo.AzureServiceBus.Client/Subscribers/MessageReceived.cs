namespace Rydo.AzureServiceBus.Client.Subscribers
{
    using System;

    public readonly struct MessageReceived
    {
        public MessageReceived(string messageId, string partitionKey, ReadOnlyMemory<byte> payload)
        {
            MessageId = messageId;
            PartitionKey = partitionKey;
            Payload = payload;
        }

        public readonly string MessageId;
        public readonly string PartitionKey;
        public readonly ReadOnlyMemory<byte> Payload;
    }
}