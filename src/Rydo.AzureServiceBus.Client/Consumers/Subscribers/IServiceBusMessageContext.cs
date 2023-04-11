namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.ServiceBus;

    public interface IServiceBusMessageContext
    {
        int DeliveryCount { get; }
        string Label { get; }
        long SequenceNumber { get; }
        long EnqueuedSequenceNumber { get; }
        string LockToken { get; }
        DateTime LockedUntil { get; }
        string SessionId { get; }
        long Size { get; }
        string To { get; }
        string ReplyToSessionId { get; }
        string PartitionKey { get; }
        string ReplyTo { get; }
        DateTime EnqueuedTime { get; }
        DateTime ScheduledEnqueueTime { get; }
        IReadOnlyDictionary<string, object> Properties { get; }
        TimeSpan TimeToLive { get; }
        string CorrelationId { get; }
        string MessageId { get; }
        DateTime ExpiresAt { get; }
        IMessageBody Body { get; }
        string ContentType { get; }
    }

    public sealed class ServiceBusMessageContext :
        IServiceBusMessageContext
    {
        internal readonly ServiceBusReceivedMessage Message;

        public ServiceBusMessageContext(ServiceBusReceivedMessage message)
        {
            Message = message;
            Body = new ServiceBusMessageBody(message.Body);
        }

        public IMessageBody Body { get; }
        public string ContentType => Message.ContentType;
        public int DeliveryCount => Message.DeliveryCount;
        public string Label => Message.Subject;
        public long SequenceNumber => Message.SequenceNumber;
        public long EnqueuedSequenceNumber => Message.EnqueuedSequenceNumber;
        public string LockToken => Message.LockToken;
        public DateTime LockedUntil => Message.LockedUntil.UtcDateTime;
        public string SessionId => Message.SessionId;
        public long Size => Message.Body.ToArray().Length;
        public string To => Message.To;
        public string ReplyToSessionId => Message.ReplyToSessionId;
        public string PartitionKey => Message.PartitionKey;
        public string ReplyTo => Message.ReplyTo;
        public DateTime EnqueuedTime => Message.EnqueuedTime.UtcDateTime;
        public DateTime ScheduledEnqueueTime => Message.ScheduledEnqueueTime.UtcDateTime;
        public IReadOnlyDictionary<string, object> Properties => Message.ApplicationProperties;
        public TimeSpan TimeToLive => Message.TimeToLive;
        public string CorrelationId => Message.CorrelationId;
        public string MessageId => Message.MessageId;
        public DateTime ExpiresAt => Message.ExpiresAt.UtcDateTime;
    }
}