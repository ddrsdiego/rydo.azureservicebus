namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Client.Consumers;
    using Handlers;
    using Microsoft.Extensions.Logging;

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
                var messageId = messageContext.Message.MessageId;
                var rawData = messageContext.Message.Payload.ToArray();

                using var streamValue = new MemoryStream(rawData);
                var messageValue = await JsonSerializer.DeserializeAsync(
                        streamValue,
                        context.ConsumerContext.ContractType, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                var messageRecord = new MessageRecord(messageId, messageValue, messageContext.ReceivedMessage);
                messageContext.SetMessageRecord(messageRecord);
            }

            await next(context);
        }
    }
}