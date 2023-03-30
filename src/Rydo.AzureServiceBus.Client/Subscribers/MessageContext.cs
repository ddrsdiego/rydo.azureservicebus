namespace Rydo.AzureServiceBus.Client.Subscribers
{
    using System;
    using Azure.Messaging.ServiceBus;
    using Handlers;

    internal sealed class MessageContext
    {
        private MessageConsumerContext _messageConsumerContext;

        public MessageContext(ServiceBusReceivedMessage receivedMessage)
        {
            ReceivedMessage = receivedMessage;
        }

        public MessageRecord Message;
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