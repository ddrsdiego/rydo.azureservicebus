﻿namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;

    internal sealed class CustomConsumerMiddleware : MessageMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private const string CustomHandlerConsumerStep = "CUSTOM-HANDLER-CONSUMER-MESSAGES";

        public CustomConsumerMiddleware(IServiceProvider serviceProvider)
            :base(nameof(CustomConsumerMiddleware))
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override string ConsumerMessagesStep => CustomHandlerConsumerStep;

        protected override async Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            using var scope = _serviceProvider.CreateScope();
            if (scope.ServiceProvider.GetService(context.HandlerType ?? throw new InvalidOperationException()) is
                IConsumerHandler messageHandler)
            {
                try
                {
                    await messageHandler.HandleAsync(context, context.CancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}