namespace Rydo.AzureServiceBus.Client.Subscribers
{
    using System;
    using Azure.Messaging.ServiceBus;
    using Handlers;

    public sealed class MessageContext
    {
        private MessageConsumerContext _messageConsumerContext;

        public MessageContext(MessageReceived message, ServiceBusReceivedMessage receivedMessage)
        {
            Message = message;
            ReceivedMessage = receivedMessage;
        }

        public readonly MessageReceived Message;
        public MessageRecord MessageRecord;
        public readonly ServiceBusReceivedMessage ReceivedMessage;

        internal void SetMessageRecord(MessageRecord messageRecord)
        {
            MessageRecord = messageRecord;
            MessageRecord.SetMessageConsumerContext(_messageConsumerContext);
        }

        internal void SetMessageConsumerContext(MessageConsumerContext context)
        {
            _messageConsumerContext = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}