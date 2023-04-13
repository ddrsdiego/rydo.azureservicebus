namespace Rydo.AzureServiceBus.Client.Middlewares
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Extensions;
    using Handlers;
    using Logging.Observers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Observers;

    internal sealed class MiddlewareExecutor : IMiddlewareExecutor
    {
        private readonly ILogger<MiddlewareExecutor> _logger;
        private readonly Dictionary<int, IMessageMiddleware> _consumerMiddlewares;
        private readonly IMiddlewareConfigurationContainer _middlewareConfigurationContainer;

        internal MiddlewareExecutor(ILogger<MiddlewareExecutor> logger,
            IMiddlewareConfigurationContainer middlewareConfigurationContainer)
        {
            _logger = logger;
            _middlewareConfigurationContainer = middlewareConfigurationContainer;
            _consumerMiddlewares = new Dictionary<int, IMessageMiddleware>();
        }

        internal static MiddlewareExecutorBuilder Builder() => new MiddlewareExecutorBuilder();

        public Task Execute(IServiceScope scope, MessageConsumerContext context,
            Func<MessageConsumerContext, Task> nextOperation)
        {
            const int startIndexPosition = 0;

            if (context == null) throw new ArgumentNullException(nameof(context));

            return ExecuteDefinition(startIndexPosition, scope, context, nextOperation);
        }

        private Task ExecuteDefinition(int index, IServiceScope scope, MessageConsumerContext context,
            Func<MessageConsumerContext, Task> nextOperation)
        {
            const int oneIncrementPosition = 1;

            MiddlewareConfiguration configuration = default;

            if (index == _middlewareConfigurationContainer?.TotalConfigurations)
                return nextOperation(context);

            if (_middlewareConfigurationContainer != null && _middlewareConfigurationContainer.Configs.Count > index)
                configuration = _middlewareConfigurationContainer.Configs[index];

            if (index == _middlewareConfigurationContainer?.Configs.Count)
                configuration = _middlewareConfigurationContainer?.FinallyProcess;

            var messageMiddleware = ResolveInstance(index, scope, configuration);

            return messageMiddleware.InvokeAsync(context,
                nextContext => ExecuteDefinition(index + oneIncrementPosition, scope, nextContext, nextOperation));
        }

        private IMessageMiddleware ResolveInstance(int index, IServiceScope scope,
            MiddlewareConfiguration configuration)
        {
            IMessageMiddleware messageMiddleware = default;

            try
            {
                messageMiddleware = _consumerMiddlewares.SafeGetOrAdd(index, _ => GetInstance(scope, configuration));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }

            return messageMiddleware;
        }

        private static IMessageMiddleware GetInstance(IServiceScope scope, MiddlewareConfiguration configuration)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var middleware = (IMessageMiddleware) scope.ServiceProvider.GetService(configuration.Type);

            middleware.ConnectConsumerObserver(new LogConsumerObserver(logger));
            middleware.ConnectConsumerMiddlewareObserver(new LogConsumerMiddlewareObserver(logger));

            return middleware;
        }
    }
}