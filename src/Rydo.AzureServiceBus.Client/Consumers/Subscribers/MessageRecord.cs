namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using Handlers;

    public interface IMessageRecord
    {
    }

    public interface IMessageRecord<out TMessage> :
        IMessageRecord
    {
        string Key { get; }
        string PartitionKey { get; }
        TMessage Value { get; }
    }

    public class MessageRecord<TMessage> : 
        IMessageRecord<TMessage>
    {
        internal MessageRecord(TMessage value, MessageRecord messageRecord)
        {
            Key = messageRecord.MessageId;
            Value = value;
            PartitionKey = messageRecord.PartitionKey;
            Message = messageRecord;
        }

        public string Key { get; }
        public string PartitionKey { get; }
        public TMessage Value { get; }
        internal MessageRecord Message { get; }

        internal MessageConsumerContext MessageConsumerCtx;

        internal void SetMessageConsumerContext(MessageConsumerContext messageConsumerContext)
        {
            MessageConsumerCtx =
                messageConsumerContext ?? throw new ArgumentNullException(nameof(messageConsumerContext));
        }
    }

    public class MessageRecord
    {
        private MessageRecord(string messageId, string partitionKey, object messageValue, bool isValid)
        {
            IsValid = isValid;
            MessageId = messageId;
            PartitionKey = partitionKey;
            MessageValue = messageValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public readonly string MessageId;

        internal readonly object MessageValue;

        /// <summary>
        /// 
        /// </summary>
        public readonly string PartitionKey;

        /// <summary>
        /// 
        /// </summary>
        internal bool IsValid { get; }

        /// <summary>
        /// 
        /// </summary>
        internal bool IsInvalid => !IsValid;

        internal MessageConsumerContext MessageConsumerCtx;

        internal static MessageRecord GetInstance(string messageId, string partitionKey, object messageValue) =>
            new MessageRecord(messageId, partitionKey, messageValue, true);

        internal static MessageRecord GetInvalidInstance(string messageId, string partitionKey) =>
            new MessageRecord(messageId, partitionKey, null, false);

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