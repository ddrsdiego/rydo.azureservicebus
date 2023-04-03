namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using Azure.Messaging.ServiceBus;
    using Handlers;

    public sealed class MessageContext
    {
        public MessageContext(ServiceBusReceivedMessage receivedMessage)
        {
            ReceivedMessage = receivedMessage;
        }

        public MessageRecord Record;
        
        internal MessageConsumerContext MessageConsumerContext;
        internal readonly ServiceBusReceivedMessage ReceivedMessage;

        internal void SetMessageRecord(MessageRecord messageRecord)
        {
            Record = messageRecord;
            Record.SetMessageConsumerContext(MessageConsumerContext);
        }

        internal void SetMessageConsumerContext(MessageConsumerContext context)
        {
            MessageConsumerContext = context ?? throw new ArgumentNullException(nameof(context));
        }
    }
}