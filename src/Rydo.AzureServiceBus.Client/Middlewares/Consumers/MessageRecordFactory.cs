namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Client.Consumers.Subscribers;
    using Microsoft.Extensions.Logging;
    using Serialization;

    internal interface IMessageRecordFactory
    {
        ValueTask<MessageRecord> ToMessageRecord(MessageContext messageContext,
            Type contractType, CancellationToken cancellationToken = default);
    }

    internal sealed class MessageRecordFactory : IMessageRecordFactory
    {
        private readonly ILogger<MessageRecordFactory> _logger;
        private readonly ISerializer _serializer;

        public MessageRecordFactory(ILogger<MessageRecordFactory> logger, ISerializer serializer)
        {
            _logger = logger;
            _serializer = serializer;
        }

        public async ValueTask<MessageRecord> ToMessageRecord(MessageContext messageContext,
            Type contractType, CancellationToken cancellationToken = default)
        {
            var messageId = messageContext.ReceivedMessage.MessageId;
            var partitionKey = messageContext.ReceivedMessage.PartitionKey;
            var rawData = messageContext.ReceivedMessage.Body.ToArray();

            MessageRecord messageRecord;

            try
            {
                var messageValue = await _serializer.DeserializeAsync(rawData, contractType, cancellationToken);
                messageRecord =
                    MessageRecord.GetInstance(messageId, partitionKey, messageValue);
            }
            catch (Exception e)
            {
                _logger.LogError("", e);
                
                messageRecord =
                    MessageRecord.GetInvalidInstance(messageId, partitionKey);
            }

            return messageRecord;
        }
    }
}