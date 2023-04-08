namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Client.Consumers.Subscribers;
    using Handlers;
    using Logging;

    internal sealed class DeserializerConsumerMiddleware : MessageMiddleware
    {
        private readonly IMessageRecordFactory _messageRecordFactory;
        
        public DeserializerConsumerMiddleware(IMessageRecordFactory messageRecordFactory)
            : base(nameof(DeserializerConsumerMiddleware))
        {
            _messageRecordFactory = messageRecordFactory;
        }

        protected override string ConsumerMessagesStep => LogTypeConstants.DeserializerConsumerMessagesStep;

        protected override async Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            var messages = context.MessagesContext.ToArray();
            for (var index = 0; index < messages.Length; index++)
            {
                var messageContext = messages[index];

                await ConsumerObservable.PreConsumerAsync(messageContext);
                
                var valueTask = _messageRecordFactory.ToMessageRecord(messageContext,
                    context.SubscriberContext.ContractType, context.CancellationToken);

                var messageRecord = valueTask.IsCompletedSuccessfully
                    ? valueTask.Result
                    : SlowAdapter(valueTask).Result;

                messageContext.SetMessageRecord(messageRecord);

                await ConsumerObservable.PostConsumerAsync(messageContext);
            }

            static async ValueTask<MessageRecord> SlowAdapter(ValueTask<MessageRecord> task)
            {
                var result = await task;
                return result;
            }
        }
    }
}