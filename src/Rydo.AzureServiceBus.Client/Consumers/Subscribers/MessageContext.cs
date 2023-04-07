namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using Azure.Messaging.ServiceBus;
    using Handlers;
    using Headers;

    internal static class ApplicationPropertiesEx
    {
        public static IMessageHeaders ExtractHeaders(this ServiceBusReceivedMessage receivedMessage)
        {
            var messageHeaders = MessageHeaders.GetInstance();
            
            foreach (var property in receivedMessage.ApplicationProperties)
                messageHeaders.SetString(property.Key, property.Value.ToString());

            return messageHeaders;
        }
    }
    
    public sealed class MessageContext
    {
        public MessageContext(ServiceBusReceivedMessage receivedMessage)
        {
            ReceivedMessage = receivedMessage;
            Headers = ReceivedMessage.ExtractHeaders();
        }

        public MessageRecord Record;
        public readonly IMessageHeaders Headers;
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