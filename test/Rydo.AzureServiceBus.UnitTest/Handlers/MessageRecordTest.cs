namespace Rydo.AzureServiceBus.UnitTest.Handlers
{
    using System;
    using NUnit.Framework;

    public class MessageRecordTest
    {
        [Test]
        public void Test()
        {
            // var messageRecord = new MessageRecord()
        }
    }

    public interface IReceivedMessage
    {
        string MessageId { get; }
        string PartitionKey { get; }
        string SessionId { get; }
        TimeSpan TimeToLive { get; }
        DateTimeOffset ExpiresAt { get; }
        DateTimeOffset EnqueuedTime { get; }
    }
}