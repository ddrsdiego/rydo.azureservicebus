namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System;
    using System.Threading.Tasks;
    using Factories;
    using Handlers;
    using Logging;

    internal sealed class CustomConsumerMiddleware : MessageMiddleware
    {
        public CustomConsumerMiddleware()
            : base(nameof(CustomConsumerMiddleware))
        {
        }

        protected override string ConsumerMessagesStep => LogTypeConstants.CustomHandlerConsumerStep;

        protected override async Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            if (context.Scope.ServiceProvider.GetService(context.HandlerType ?? throw new InvalidOperationException())
                is IConsumerHandler messageHandler)
            {
                try
                {
                    var consumerContext = ConsumerContextFactory
                        .GetConsumerContext(context.ContractType)
                        .Execute(context);

                    await ConsumerHandlerFactory
                        .GetExecutor(context.ContractType)
                        .Execute(messageHandler, consumerContext);
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}