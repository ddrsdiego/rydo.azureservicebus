namespace Rydo.AzureServiceBus.Client.Consumers.MessageRecordModel.Observers
{
    using System;
    using System.Threading.Tasks;
    using Headers;
    using Logging;
    using Microsoft.Extensions.Logging;
    using Subscribers;

    internal sealed class LogMessageRecordAdapterObserver : IMessageRecordAdapterObserver
    {
        private readonly ILogger<LogMessageRecordAdapterObserver> _logger;

        public LogMessageRecordAdapterObserver(ILoggerFactory logger) =>
            _logger = logger.CreateLogger<LogMessageRecordAdapterObserver>();

        public Task PreAdapter(IMessageContext messageContext, Type contractType)
        {
            var message = messageContext as MessageContext;
            var producer = message.Headers.GetString("producer");

            return Task.CompletedTask;
        }

        public Task FaultAdapter(IMessageContext messageContext, Type contractType, Exception exception)
        {
            var message = messageContext as MessageContext;
            var messageAudit = new MessageContextFaultAudit(message, contractType);
            
            _logger.LogError(exception, $"{ServiceBusLogFields.LogType} - {{MessageContextFaultAudit}}",
                "fault-adapter-deserialization",
                messageAudit);

            return Task.CompletedTask;
        }

        private sealed class MessageContextFaultAudit
        {
            private readonly Type _contractType;
            private readonly MessageContext _messageContext;
            
            public MessageContextFaultAudit(MessageContext messageContext, Type contractType)
            {
                _messageContext = messageContext;
                _contractType = contractType;
            }

            public string MessageId => _messageContext.Message.MessageId;
            public string PartitionKey => _messageContext.Message.PartitionKey;
            public string Contract => _contractType.Name;
            public string Producer => _messageContext.Headers.GetString("producer");
        }
    }
}