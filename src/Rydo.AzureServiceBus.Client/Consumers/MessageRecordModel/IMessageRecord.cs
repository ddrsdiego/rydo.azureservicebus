namespace Rydo.AzureServiceBus.Client.Consumers.MessageRecordModel
{
    using System;

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
}