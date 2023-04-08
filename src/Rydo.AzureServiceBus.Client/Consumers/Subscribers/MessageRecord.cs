﻿namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using Azure.Messaging.ServiceBus;
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

        internal MessageConsumerContext MessageConsumerCtx => Message.MessageConsumerCtx;
    }

    public sealed class MessageRecord : IMessageRecord
    {
        private MessageRecord(object messageValue, bool isValid, ServiceBusReceivedMessage receivedMessage)
        {
            IsValid = isValid;
            MessageId = receivedMessage.MessageId;
            PartitionKey = receivedMessage.PartitionKey;
            MessageValue = messageValue;
            SentTime = receivedMessage.EnqueuedTime;
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

        internal static MessageRecord GetInstance(object messageValue, ServiceBusReceivedMessage receivedMessage) =>
            new MessageRecord(messageValue, true, receivedMessage);

        internal static MessageRecord GetInvalidInstance(ServiceBusReceivedMessage receivedMessage) =>
            new MessageRecord(null, false, receivedMessage);

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