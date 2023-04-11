namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using Handlers;
    using Headers;
    using MessageRecordModel;

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

    public interface IMessageContext
    {
    }

    public sealed class MessageContext : 
        IMessageContext
    {
        internal MessageContext(IServiceBusMessageContext message)
        {
            Message = message;
            Headers = Message.ExtractHeaders();
        }

        internal MessageRecord Record;
        internal readonly IMessageHeaders Headers;
        internal MessageConsumerContext MessageConsumerContext;
        internal readonly IServiceBusMessageContext Message;
        
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