namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Client.Consumers.Subscribers;
    using Handlers;

    internal sealed class DeserializerConsumerMiddleware : MessageMiddleware
    {
        private readonly IMessageRecordFactory _messageRecordFactory;
        private const string DeserializerConsumerMessagesStep = "DESERIALIZER-CONSUMER-MESSAGES";

        public DeserializerConsumerMiddleware(IMessageRecordFactory messageRecordFactory)
            : base(nameof(DeserializerConsumerMiddleware))
        {
            _messageRecordFactory = messageRecordFactory;
        }

        protected override string ConsumerMessagesStep => DeserializerConsumerMessagesStep;

        protected override async Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
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

            static async ValueTask<MessageRecord> SlowAdapter(ValueTask<MessageRecord> task)
            {
                var result = await task;
                return result;
            }
        }
    }
}