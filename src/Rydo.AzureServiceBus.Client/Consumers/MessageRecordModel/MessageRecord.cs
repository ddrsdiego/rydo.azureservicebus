namespace Rydo.AzureServiceBus.Client.Consumers.MessageRecordModel
{
    using System;
    using Handlers;
    using Subscribers;

    public sealed class MessageRecord<TMessage> :
        IMessageRecord<TMessage>
        where TMessage : class
    {
        internal readonly MessageRecord Message;

        internal MessageRecord(TMessage value, MessageRecord message)
        {
            Value = value;
            Message = message;
        }

        public TMessage Value { get; }
        public string MessageId => Message.MessageId;
        public string PartitionKey => Message.PartitionKey;
        public DateTimeOffset SentTime => Message.SentTime;
    }

    public sealed class MessageRecord :
        IMessageRecord
    {
        private MessageRecord(object messageValue, bool isValid, IServiceBusMessageContext serviceBusMessageContext)
        {
            IsValid = isValid;
            MessageValue = messageValue;
            MessageId = serviceBusMessageContext.MessageId;
            PartitionKey = serviceBusMessageContext.PartitionKey;
            SentTime = serviceBusMessageContext.EnqueuedTime;
        }

        internal IMessageRecord<TMessage> GetMessageRecordTyped<TMessage>()
            where TMessage : class => new MessageRecord<TMessage>((TMessage) MessageValue, this);

        public string MessageId { get; }
        public string PartitionKey { get; }
        public DateTimeOffset SentTime { get; }

        internal readonly object MessageValue;

        /// <summary>
        /// 
        /// </summary>
        internal bool IsValid { get; }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsInvalid => !IsValid;

        internal MessageConsumerContext MessageConsumerCtx;

        internal static MessageRecord GetInstance(object messageValue,
            IServiceBusMessageContext serviceBusMessageContext) =>
            new MessageRecord(messageValue, true, serviceBusMessageContext);

        internal static MessageRecord GetInvalidInstance(IServiceBusMessageContext serviceBusMessageContext) =>
            new MessageRecord(null, false, serviceBusMessageContext);

        internal void SetMessageConsumerContext(MessageConsumerContext messageConsumerContext)
        {
            MessageConsumerCtx =
                messageConsumerContext ?? throw new ArgumentNullException(nameof(messageConsumerContext));
        }
    }
}