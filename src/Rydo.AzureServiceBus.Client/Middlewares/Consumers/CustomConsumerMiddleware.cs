namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    internal sealed class CustomConsumerMiddleware : MessageMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private const string CustomHandlerConsumerStep = "CUSTOM-HANDLER-CONSUMER-MESSAGES";

        public CustomConsumerMiddleware(ILoggerFactory logger, IServiceProvider serviceProvider)
            : base(logger.CreateLogger<CustomConsumerMiddleware>())
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            if (!context.AnyMessage)
            {
                await next(context);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            if (scope.ServiceProvider.GetService(context.HandlerType ?? throw new InvalidOperationException()) is
                IConsumerHandler messageHandler)
            {
                if (ConsumerMiddlewareObservable.Count > 0)
                    await ConsumerMiddlewareObservable.PreConsumer(nameof(CustomConsumerMiddleware),
                        CustomHandlerConsumerStep, context);

                try
                {
                    await messageHandler.HandleAsync(context, context.CancellationToken);

                    if (ConsumerMiddlewareObservable.Count > 0)
                        await ConsumerMiddlewareObservable.PostConsumer(nameof(CustomConsumerMiddleware),
                            CustomHandlerConsumerStep, context);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            await next(context);
        }
    }
}