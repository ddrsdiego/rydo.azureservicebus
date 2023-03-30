namespace Rydo.AzureServiceBus.Client.Subscribers
{
    using System;
    using Azure.Messaging.ServiceBus;
    using Handlers;

    public sealed class MessageContext
    {
        private MessageConsumerContext _messageConsumerContext;

        public MessageContext(MessageReceived messageReceived, ServiceBusReceivedMessage receivedMessage)
        {
            MessageReceived = messageReceived;
            ReceivedMessage = receivedMessage;
        }

        public MessageRecord Message;
        internal readonly MessageReceived MessageReceived;
        internal readonly ServiceBusReceivedMessage ReceivedMessage;

        internal void SetMessageRecord(MessageRecord messageRecord)
        {
            Message = messageRecord;
            Message.SetMessageConsumerContext(_messageConsumerContext);
        }

        internal void SetMessageConsumerContext(MessageConsumerContext context)
        {
            _messageConsumerContext = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}