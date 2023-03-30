namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    internal sealed class CustomConsumerMiddleware : MessageMiddleware
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomConsumerMiddleware(ILoggerFactory logger, IServiceProvider serviceProvider)
            : base(logger.CreateLogger<CustomConsumerMiddleware>())
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            if (!context.Messages.Any())
            {
                await next(context);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            if (scope.ServiceProvider.GetService(context.HandlerType ?? throw new InvalidOperationException()) is
                IConsumerHandler messageHandler)
            {
                await messageHandler.HandleAsync(context, context.CancellationToken);
            }

            await next(context);
        }
    }
}