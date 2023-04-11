namespace Rydo.AzureServiceBus.Client.Logging.Observers
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Consumers.Subscribers;
    using Headers;
    using Microsoft.Extensions.Logging;

    internal sealed class LogReceiveObserver : IReceiveObserver
    {
        private readonly ILogger<LogReceiveObserver> _logger;

        public LogReceiveObserver(ILoggerFactory logger) => _logger = logger.CreateLogger<LogReceiveObserver>();

        public Task PreStartReceive(SubscriberContext context)
        {
            _logger.LogDebug($"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.SubscriberContextLog}",
                LogTypeConstants.StartReceiver,
                new SubscriberContextLog(context));

            return Task.CompletedTask;
        }

        public Task PostStartReceive(SubscriberContext context)
        {
            var log = new SubscriberContextLog(context);

            _logger.LogInformation($"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.SubscriberContextLog}",
                LogTypeConstants.ConnectedReceiver,
                log);

            return Task.CompletedTask;
        }

        public Task FaultStartReceive(SubscriberContext context, Exception exception)
        {
            var log = new SubscriberContextLog(context);

            _logger.LogError(exception,
                $"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.SubscriberContextLog}",
                LogTypeConstants.ConnectedReceiver,
                log);

            return Task.CompletedTask;
        }

        public Task PreReceiveAsync(MessageContext context)
        {
            _logger.LogDebug($"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MessageContextLog}",
                LogTypeConstants.IncomingMessageReceiver,
                MessageContextLog.GetInstance(context));

            return Task.CompletedTask;
        }
    }

    internal sealed class MessageContextLog
    {
        private readonly MessageContext _context;

        private MessageContextLog(MessageContext context) => _context = context;

        public static MessageContextLog GetInstance(MessageContext context) => new MessageContextLog(context);

        public string ContextId => _context.MessageConsumerContext.ContextId;
        public string MessageId => _context.Message.MessageId;
        public string ContentType => _context.Message.ContentType;
        public string PartitionKey => _context.Message.PartitionKey;
        public string Topic => _context.MessageConsumerContext.SubscriberContext.Specification.TopicName;
        public string Subscription => _context.MessageConsumerContext.SubscriberContext.Specification.SubscriptionName;
        public string Queue => _context.MessageConsumerContext.SubscriberContext.Specification.QueueName;
        public string Producer => _context.Headers.GetString("producer");
    }

    internal sealed class SubscriberContextLog
    {
        private readonly SubscriberContext _context;

        internal SubscriberContextLog(SubscriberContext context) => _context = context;

        public string TopicName => _context.Specification.TopicName;
        public string SubscriptionName => _context.Specification.SubscriptionName;
        public string Handler => _context.HandlerType.Name;
        public string Contract => _context.ContractType.Name;
        public string QueueSubscription => _context.Specification.QueueName;
        public int PrefetchCount => _context.Specification.PrefetchCount;
        public int MaxMessages => _context.Specification.MaxMessages;
        public int MaxDeliveryCount => _context.Specification.MaxDeliveryCount;
        public string ReceiveMode => _context.Specification.ReceiveMode.ToString();

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}