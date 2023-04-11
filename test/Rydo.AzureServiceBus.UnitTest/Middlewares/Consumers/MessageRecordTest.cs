namespace Rydo.AzureServiceBus.UnitTest.Middlewares.Consumers
{
    using System;
    using System.Text.Json;
    using Client.Consumers.MessageRecordModel;
    using Client.Consumers.Subscribers;
    using Consumer.Models;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;

    public class MessageRecordTest
    {
        [Test]
        public void Should_Create_Valid_MessageRecord_Typed()
        {
            var accountCreated = new AccountCreated("5090016");
            var payload = JsonSerializer.SerializeToUtf8Bytes(accountCreated);

            var serviceBusMessageContext = Substitute.For<IServiceBusMessageContext>();

            var sentTime = DateTime.UtcNow;
            var messageId = Guid.NewGuid().ToString();
            var partitionKey = Guid.NewGuid().ToString();

            serviceBusMessageContext.EnqueuedTime.Returns(sentTime);
            serviceBusMessageContext.MessageId.Returns(messageId);
            serviceBusMessageContext.PartitionKey.Returns(partitionKey);
            
            serviceBusMessageContext.Body.Returns(new ServiceBusMessageBody(new BinaryData(payload)));

            var messageRecord = MessageRecord.GetInstance(accountCreated, serviceBusMessageContext);
            var messageRecordTyped = messageRecord.GetMessageRecordTyped<AccountCreated>();

            messageRecordTyped.MessageId.Should().Be(messageId);
            messageRecordTyped.SentTime.Should().Be(sentTime);
            messageRecordTyped.PartitionKey.Should().Be(partitionKey);
        }
        
        [Test]
        public void Should_Create_Valid_MessageRecord()
        {
            var accountCreated = new AccountCreated("5090016");
            var payload = JsonSerializer.SerializeToUtf8Bytes(accountCreated);

            var serviceBusMessageContext = Substitute.For<IServiceBusMessageContext>();

            var sentTime = DateTime.UtcNow;
            var messageId = Guid.NewGuid().ToString();
            var partitionKey = Guid.NewGuid().ToString();

            serviceBusMessageContext.EnqueuedTime.Returns(sentTime);
            serviceBusMessageContext.MessageId.Returns(messageId);
            serviceBusMessageContext.PartitionKey.Returns(partitionKey);
            
            serviceBusMessageContext.Body.Returns(new ServiceBusMessageBody(new BinaryData(payload)));

            var messageRecord = MessageRecord.GetInstance(accountCreated, serviceBusMessageContext);
            
            messageRecord.IsValid.Should().BeTrue();
            messageRecord.MessageId.Should().Be(messageId);
            messageRecord.PartitionKey.Should().Be(partitionKey);
            messageRecord.SentTime.Should().Be(sentTime);
        }
    }
}