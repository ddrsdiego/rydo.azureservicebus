namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System.Threading.Tasks;
    using Client.Consumers.MessageRecordModel;
    using Client.Consumers.Subscribers;
    using Handlers;
    using Logging;

    internal sealed class DeserializerConsumerMiddleware : MessageMiddleware
    {
        private readonly IMessageRecordAdapter _messageRecordAdapter;

        public DeserializerConsumerMiddleware(IMessageRecordAdapter messageRecordAdapter)
            : base(nameof(DeserializerConsumerMiddleware))
        {
            _messageRecordAdapter = messageRecordAdapter;
        }

        protected override string ConsumerMessagesStep => LogTypeConstants.DeserializerConsumerMessagesStep;

        protected override async Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            for (var index = 0; index < context.MessagesContext.Length; index++)
            {
                var messageContext = context.MessagesContext[index] as MessageContext;
                
                await ConsumerObservable.PreConsumerAsync(messageContext);

                var valueTask = _messageRecordAdapter.ToMessageRecord(messageContext,
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