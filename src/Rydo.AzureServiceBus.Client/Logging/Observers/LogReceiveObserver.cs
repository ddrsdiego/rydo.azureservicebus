namespace Rydo.AzureServiceBus.Client.Logging.Observers
{
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Consumers.Subscribers;
    using Microsoft.Extensions.Logging;

    internal sealed class LogReceiveObserver : IReceiveObserver
    {
        private const string StartReceiverLogType = "START-RECEIVER";
        private const string ConnectedReceiverLogType = "CONNECTED-RECEIVER";
        private const string IncomingMessageReceiverLogType = "INCOMING-MESSAGE";
        
        private readonly ILogger<LogReceiveObserver> _logger;

        public LogReceiveObserver(ILogger<LogReceiveObserver> logger)
        {
            _logger = logger;
        }
        
        public Task PreStartReceive(SubscriberContext context)
        {
            _logger.LogInformation($"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.SubscriberContextLog}",
                StartReceiverLogType,
                new SubscriberContextLog(context));
            
            return Task.CompletedTask;
        }

        public Task PostStartReceive(SubscriberContext context)
        {
            _logger.LogInformation($"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.SubscriberContextLog}",
                ConnectedReceiverLogType,
                new SubscriberContextLog(context));
            
            return Task.CompletedTask;
        }

        public Task PreReceive(MessageContext context)
        {
            _logger.LogDebug($"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MessageContextLog}",
                IncomingMessageReceiverLogType,
                new MessageContextLog(context));
            
            return Task.CompletedTask;
        }
    }

    internal sealed class MessageContextLog
    {
        private readonly MessageContext _context;

        public MessageContextLog(MessageContext context)
        {
            _context = context;
        }

        public string MessageId => _context.ReceivedMessage.MessageId;
        public string ContentType => _context.ReceivedMessage.ContentType;
        public string PartitionKey => _context.ReceivedMessage.PartitionKey;
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