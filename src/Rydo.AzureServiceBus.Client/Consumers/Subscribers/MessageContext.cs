namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using Azure.Messaging.ServiceBus;
    using Handlers;
    using Headers;

    internal static class ApplicationPropertiesEx
    {
        public static IMessageHeaders ExtractHeaders(this IServiceBusMessageContext receivedMessage)
        {
            var messageHeaders = MessageHeaders.GetInstance();
            
            foreach (var property in receivedMessage.Properties)
                messageHeaders.SetString(property.Key, property.Value.ToString());

            return messageHeaders;
        }
    }
    
    public sealed class MessageContext
    {
        internal MessageContext(ServiceBusMessageContext serviceBusMessageContext)
        {
            ServiceBusMessageContext = serviceBusMessageContext;
            Headers = ServiceBusMessageContext.ExtractHeaders();
        }

        internal MessageRecord Record;
        internal readonly IMessageHeaders Headers;
        internal MessageConsumerContext MessageConsumerContext;
        internal readonly ServiceBusMessageContext ServiceBusMessageContext;
        
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