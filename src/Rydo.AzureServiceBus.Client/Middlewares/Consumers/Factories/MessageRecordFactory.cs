namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers.Factories
{
    using System;
    using System.Collections.Concurrent;
    using Client.Consumers.Subscribers;

    public abstract class MessageRecordFactory
    {
        private static readonly ConcurrentDictionary<Type, MessageRecordFactory> Executors =
            new ConcurrentDictionary<Type, MessageRecordFactory>();

        internal static MessageRecordFactory GetConsumerContext(Type messageType)
        {
            var messageRecordFactory = Executors.GetOrAdd(messageType,
                _ => (MessageRecordFactory) Activator.CreateInstance(
                    typeof(InnerMessageRecord<>).MakeGenericType(messageType)));

            return messageRecordFactory;
        }

        public abstract IMessageRecord Execute(MessageRecord messageRecord);

        private class InnerMessageRecord<TMessage> : MessageRecordFactory
            where TMessage : class
        {
            public override IMessageRecord Execute(MessageRecord messageRecord)
            {
                var typedMessageRecord =
                    new MessageRecord<TMessage>((TMessage) messageRecord.MessageValue, messageRecord);

                return typedMessageRecord;
            }
        }
    }
}