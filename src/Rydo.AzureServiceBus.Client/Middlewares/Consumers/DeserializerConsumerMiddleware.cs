namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Client.Consumers.Subscribers;
    using Handlers;
    using Microsoft.Extensions.Logging;

    internal sealed class DeserializerConsumerMiddleware : MessageMiddleware
    {
        private readonly IMessageRecordFactory _messageRecordFactory;
        private const string DeserializerConsumerMessagesStep = "DESERIALIZER-CONSUMER-MESSAGES";
        
        public DeserializerConsumerMiddleware(ILoggerFactory logger, IMessageRecordFactory messageRecordFactory)
            : base(logger.CreateLogger<DeserializerConsumerMiddleware>())
        {
            _messageRecordFactory = messageRecordFactory;
        }

        public override async Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            if (!context.AnyMessage)
                await next(context);

            if (ConsumerMiddlewareObservable.Count > 0)
                await ConsumerMiddlewareObservable.PreConsumer(nameof(DeserializerConsumerMiddleware),
                    DeserializerConsumerMessagesStep, context);
            
            var messages = context.MessagesContext.ToArray();
            for (var index = 0; index < messages.Length; index++)
            {
                var messageContext = messages[index];

                if (ConsumerObservable.Count > 0)
                    await ConsumerObservable.PreConsumer(messageContext);

                var valueTask = _messageRecordFactory.ToMessageRecord(messageContext,
                    context.SubscriberContext.ContractType, context.CancellationToken);

                var messageRecord = valueTask.IsCompletedSuccessfully
                    ? valueTask.Result
                    : SlowAdapter(valueTask).Result;

                messageContext.SetMessageRecord(messageRecord);

                if (ConsumerObservable.Count > 0)
                    await ConsumerObservable.PostConsumer(messageContext);
            }

            if (ConsumerMiddlewareObservable.Count > 0)
                await ConsumerMiddlewareObservable.PostConsumer(nameof(DeserializerConsumerMiddleware),
                    DeserializerConsumerMessagesStep, context);
            
            await next(context);

            static async ValueTask<MessageRecord> SlowAdapter(ValueTask<MessageRecord> task)
            {
                var result = await task;
                return result;
            }
        }
    }
}