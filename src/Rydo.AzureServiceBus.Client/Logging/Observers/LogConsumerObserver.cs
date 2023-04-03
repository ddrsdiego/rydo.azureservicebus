namespace Rydo.AzureServiceBus.Client.Logging.Observers
{
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Consumers.Subscribers;
    using Handlers;
    using Microsoft.Extensions.Logging;

    internal sealed class LogConsumerObserver : IConsumerObserver
    {
        private readonly ILogger<LogConsumerObserver> _logger;

        public LogConsumerObserver(ILoggerFactory logger) => _logger = logger.CreateLogger<LogConsumerObserver>();

        public Task PreConsumerAsync(MessageContext context) => Task.CompletedTask;

        public Task PreConsumer(MessageConsumerContext context) => Task.CompletedTask;

        public Task PostConsumerAsync(MessageContext context) => Task.CompletedTask;

        public Task PostConsumer(MessageConsumerContext context)
        {
            _logger.LogInformation(
                $"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MessageConsumerContextLength} messages completed in {ServiceBusLogFields.ElapsedMilliseconds} ms.",
                "COMPLETED_MESSAGE",
                context.Length,
                context.ElapsedTimeMiddleware);

            return Task.CompletedTask;
        }
    }
}