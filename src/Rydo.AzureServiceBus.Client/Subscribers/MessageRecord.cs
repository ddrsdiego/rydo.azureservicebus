namespace Rydo.AzureServiceBus.Client.Subscribers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using Azure.Messaging.ServiceBus;
    using Handlers;

    public sealed class MessageRecord
    {
        private readonly object _messageValue;
        private readonly byte[] _key;
        private readonly ReadOnlyMemory<byte> _value;

        public MessageRecord(string messageId, string partitionKey, object messageValue, ServiceBusReceivedMessage receivedMessage)
        {
            MessageId = messageId;
            _messageValue = messageValue;
            _value = receivedMessage.Body.ToArray();
        }

        public readonly string MessageId;

        internal bool IsValid { get; set; }

        internal MessageConsumerContext MessageConsumerCtx;

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