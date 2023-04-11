namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System;
    using System.Threading.Tasks;
    using Factories;
    using Handlers;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class CustomConsumerMiddleware : MessageMiddleware
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomConsumerMiddleware(IServiceProvider serviceProvider)
            : base(nameof(CustomConsumerMiddleware))
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override string ConsumerMessagesStep => LogTypeConstants.CustomHandlerConsumerStep;

        protected override async Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            using var scope = _serviceProvider.CreateScope();
            if (scope.ServiceProvider.GetService(context.HandlerType ?? throw new InvalidOperationException()) is
                IConsumerHandler messageHandler)
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