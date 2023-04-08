namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using Handlers;

    public interface IMessageRecord
    {
        string MessageId { get; }
        string PartitionKey { get; }
        DateTimeOffset SentTime { get; }
    }

    public interface IMessageRecord<out TMessage> :
        IMessageRecord
        where TMessage : class
    {
        TMessage Value { get; }
    }

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

        /// <summary>
        /// Get the raw message contained in the Value field
        /// </summary>
        /// <typeparam name="T">The target type of the JSON contains in value field.</typeparam>
        /// <returns>A T representation of the JSON value.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Value<T>()
        {
            if (MessageValue is null) return default;

            try
            {
                return (T) MessageValue;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ValueAsJsonString()
        {
            var valueAsJsonString =
                JsonSerializer.Serialize(MessageValue);
            return valueAsJsonString;
        }

        internal void SetMessageConsumerContext(MessageConsumerContext messageConsumerContext)
        {
            MessageConsumerCtx =
                messageConsumerContext ?? throw new ArgumentNullException(nameof(messageConsumerContext));
        }
    }
}