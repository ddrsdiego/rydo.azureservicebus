namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System.Threading.Tasks;
    using Handlers;

    internal sealed class DeadLetterHandleMiddleware : MessageMiddleware
    {
        public DeadLetterHandleMiddleware()
            : base(nameof(DeadLetterHandleMiddleware))
        {
        }

        private const string DeadLetterConsumerStep = "DEAD-LETTER-CONSUMER-MESSAGES";

        protected override string ConsumerMessagesStep => DeadLetterConsumerStep;

        protected override Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            return Task.CompletedTask;
        }
    }
}