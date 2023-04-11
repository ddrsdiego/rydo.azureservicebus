namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System.Threading.Tasks;
    using Handlers;
    using Logging;

    internal sealed class DeadLetterHandleMiddleware : MessageMiddleware
    {
        public DeadLetterHandleMiddleware()
            : base(nameof(DeadLetterHandleMiddleware))
        {
        }

        protected override string ConsumerMessagesStep => LogTypeConstants.DeadLetterConsumerStep;

        protected override async Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            await Task.CompletedTask;
        }
    }
}