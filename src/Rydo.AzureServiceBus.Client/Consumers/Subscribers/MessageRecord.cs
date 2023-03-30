namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using Azure.Messaging.ServiceBus;
    using Handlers;

    public sealed class MessageRecord
    {
        private readonly object _messageValue;

        private MessageRecord(string messageId, string partitionKey, object messageValue, bool isValid,
            ServiceBusReceivedMessage receivedMessage)
        {
            IsValid = isValid;
            MessageId = messageId;
            PartitionKey = partitionKey;
            _messageValue = messageValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public readonly string MessageId;
        
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

        internal static MessageRecord GetInstance(string messageId, string partitionKey, object messageValue,
            ServiceBusReceivedMessage receivedMessage) =>
            new MessageRecord(messageId, partitionKey, messageValue, true, receivedMessage);
        
        internal static MessageRecord GetInvalidInstance(string messageId, string partitionKey, object messageValue,
            ServiceBusReceivedMessage receivedMessage) =>
            new MessageRecord(messageId, partitionKey, null, false, receivedMessage);

        /// <summary>
        /// Get the raw message contained in the Value field
        /// </summary>
        /// <typeparam name="T">The target type of the JSON contains in value field.</typeparam>
        /// <returns>A T representation of the JSON value.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Value<T>()
        {
            if (_messageValue is null) return default;

            try
            {
                return (T) _messageValue;
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
                JsonSerializer.Serialize(_messageValue);
            return valueAsJsonString;
        }

        internal void SetMessageConsumerContext(MessageConsumerContext messageConsumerContext)
        {
            MessageConsumerCtx =
                messageConsumerContext ?? throw new ArgumentNullException(nameof(messageConsumerContext));
        }
    }
}