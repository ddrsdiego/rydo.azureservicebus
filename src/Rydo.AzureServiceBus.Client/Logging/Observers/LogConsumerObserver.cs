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

        public LogConsumerObserver(ILogger<LogConsumerObserver> logger)
        {
            _logger = logger;
        }

        public Task PreConsumer(MessageContext context)
        {
            return Task.CompletedTask;
        }

        public Task PreConsumer(MessageConsumerContext context)
        {
            return Task.CompletedTask;
        }

        public Task PostConsumer(MessageContext context)
        {
            return Task.CompletedTask;
        }

        public Task PostConsumer(MessageConsumerContext context)
        {
            _logger.LogInformation(
                $"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MessageConsumerContextLength} messages completed in {ServiceBusLogFields.ElapsedMilliseconds} ms.",
                "COMPLETED_MESSAGE",
                context.Count,
                context.ElapsedTimeConsumer);

            return Task.CompletedTask;
        }
    }
}