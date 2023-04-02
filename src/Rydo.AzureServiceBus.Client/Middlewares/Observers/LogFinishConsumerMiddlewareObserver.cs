namespace Rydo.AzureServiceBus.Client.Middlewares.Observers
{
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.Logging;

    internal sealed class LogFinishConsumerMiddlewareObserver : IFinishConsumerMiddlewareObserver
    {
        private readonly ILogger _logger;

        public LogFinishConsumerMiddlewareObserver(ILoggerFactory logger) =>
            _logger = logger.CreateLogger<LogFinishConsumerMiddlewareObserver>();

        public Task EndConsumerAsync(MessageConsumerContext context)
        {
            context.StopMsgContextWatch();
            
            _logger.LogInformation($"Passou aqui..... {context.ElapsedTimeMessageContext}");
            return Task.CompletedTask;
        }
    }
}