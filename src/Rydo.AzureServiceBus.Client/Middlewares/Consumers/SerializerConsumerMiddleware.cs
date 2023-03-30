namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.Logging;
    using Subscribers;

    internal sealed class SerializerConsumerMiddleware : MessageMiddleware
    {
        public SerializerConsumerMiddleware(ILoggerFactory logger)
            : base(logger.CreateLogger<SerializerConsumerMiddleware>())
        {
        }

        public override async Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            foreach (var messageContext in context.MessageContexts)
            {
                var valueTask = messageContext.ToMessageRecord(context.SubscriverContext.ContractType,
                    context.CancellationToken);

                var messageRecord = valueTask.IsCompletedSuccessfully
                    ? valueTask.Result 
                    : SlowAdapter(valueTask).Result;
                
                messageContext.SetMessageRecord(messageRecord);
            }

            await next(context);

            static async ValueTask<MessageRecord> SlowAdapter(ValueTask<MessageRecord> task)
            {
                var result = await task;
                return result;
            }
        }
    }

    internal static class MessageContextAdapterEx
    {
        public static async ValueTask<MessageRecord> ToMessageRecord(this MessageContext messageContext,
            Type contractType, CancellationToken cancellationToken = default)
        {
            var messageId = messageContext.ReceivedMessage.MessageId;
            var partitionKey = messageContext.ReceivedMessage.PartitionKey;
            var rawData = messageContext.ReceivedMessage.Body.ToArray();

            using var streamValue = new MemoryStream(rawData);
            var messageValue = await JsonSerializer.DeserializeAsync(
                    streamValue,
                    contractType, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var messageRecord =
                MessageRecord.GetInstance(messageId, partitionKey, messageValue, messageContext.ReceivedMessage);

            return messageRecord;
        }
    }
}