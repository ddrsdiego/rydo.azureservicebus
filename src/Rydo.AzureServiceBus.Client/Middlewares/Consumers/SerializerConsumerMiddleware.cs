namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
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

        public override async Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next,
            CancellationToken cancellationToken = default)
        {
            foreach (var messageContext in context.MessageContexts)
            {
                var messageId = messageContext.ReceivedMessage.MessageId;
                var partitionKey = messageContext.ReceivedMessage.PartitionKey;
                var rawData = messageContext.ReceivedMessage.Body.ToArray();

                using var streamValue = new MemoryStream(rawData);
                var messageValue = await JsonSerializer.DeserializeAsync(
                        streamValue,
                        context.ConsumerContext.ContractType, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                var messageRecord =
                    new MessageRecord(messageId, partitionKey, messageValue, messageContext.ReceivedMessage);

                messageContext.SetMessageRecord(messageRecord);
            }

            await next(context);
        }
    }
}