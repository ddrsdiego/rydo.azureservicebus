namespace Rydo.AzureServiceBus.Client.Abstractions.Observers
{
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus.Administration;
    using Consumers.Subscribers;
    using Logging;
    using Microsoft.Extensions.Logging;

    internal sealed class LogAdminClientObserver : IAdminClientObserver
    {
        private ILogger<LogAdminClientObserver> _logger;

        public LogAdminClientObserver(ILoggerFactory loggerFactory) => _logger = loggerFactory.CreateLogger<LogAdminClientObserver>();

        public Task VerifyQueueExitsAsync(CreateQueueOptions queueOptions)
        {
            _logger.LogInformation(
                $"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MsgConsumerContextAuditMedata}",
                "VERIFY-QUEUE-EXISTS",
                queueOptions);

            return Task.CompletedTask;
        }

        public Task PreConsumerAsync(SubscriberContext context)
        {
            return  Task.CompletedTask;
        }
    }
}