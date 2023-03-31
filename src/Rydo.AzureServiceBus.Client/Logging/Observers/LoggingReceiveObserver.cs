namespace Rydo.AzureServiceBus.Client.Logging.Observers
{
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Consumers.Subscribers;
    using Microsoft.Extensions.Logging;

    internal sealed class LoggingReceiveObserver : IReceiveObserver
    {
        private const string StartReceiverLogType = "START-RECEIVER";
        private const string ConnectedReceiverLogType = "CONNECTED-RECEIVER";
        
        private readonly ILogger<LoggingReceiveObserver> _logger;

        public LoggingReceiveObserver(ILogger<LoggingReceiveObserver> logger)
        {
            _logger = logger;
        }
        
        public Task PreStartReceive(SubscriberContext context)
        {
            _logger.LogInformation($"{ServiceBusLogFields.LogType} - {ServiceBusLogFields.SubscriberContextLog}",
                StartReceiverLogType,
                new SubscriberContextLog(context));
            
            return Task.CompletedTask;
        }

        public Task PostStartReceive(SubscriberContext context)
        {
            _logger.LogInformation($"{ServiceBusLogFields.LogType} - {ServiceBusLogFields.SubscriberContextLog}",
                ConnectedReceiverLogType,
                new SubscriberContextLog(context));
            
            return Task.CompletedTask;
        }
    }

    internal sealed class SubscriberContextLog
    {
        private readonly SubscriberContext _context;

        public SubscriberContextLog(SubscriberContext context) => _context = context;

        public string TopicName => _context.SubscriberSpecification.TopicName;
        public string SubscriptionName => _context.SubscriberSpecification.SubscriptionName;
        public string Handler => _context.HandlerType.Name;
        public string Contract => _context.ContractType.Name;
        public string QueueSubscription => _context.QueueSubscription;
    }
}