namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using Azure.Messaging.ServiceBus;
    using Handlers;

    public sealed class MessageContext
    {
        private MessageConsumerContext _messageConsumerContext;

        public MessageContext(ServiceBusReceivedMessage receivedMessage)
        {
            ReceivedMessage = receivedMessage;
        }

        public MessageRecord Record;
        
        internal readonly ServiceBusReceivedMessage ReceivedMessage;

        internal void SetMessageRecord(MessageRecord messageRecord)
        {
            Record = messageRecord;
            Record.SetMessageConsumerContext(_messageConsumerContext);
        }

        internal void SetMessageConsumerContext(MessageConsumerContext context)
        {
            _messageConsumerContext = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}