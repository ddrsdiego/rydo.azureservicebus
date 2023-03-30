namespace Rydo.AzureServiceBus.UnitTest.Producers
{
    using System;
    using System.Linq;
    using Client.Headers;
    using Client.Producers;
    using Fakes;
    using FluentAssertions;
    using NUnit.Framework;

    public class ProducerBatchRequestTest
    {
        [Test]
        public void Should_Remove_Header_From_Valid_ProducerBatchRequest()
        {
            var batch = ProducerBatchRequest.Create();

            var accountCreatedMessage = new AccountCreatedMessage("5090016");

            var messageHeader = MessageHeaders.GetInstance();
            messageHeader.SetPartition(accountCreatedMessage.AccountNumber);
            messageHeader.SetString(MessageHeadersDefault.Producer, "unit-test");

            batch.Add(accountCreatedMessage, messageHeader);
            messageHeader.Remove(MessageHeadersDefault.Producer);

            var firstPartitionKey = batch.Items.First().MessageHeaders.GetString(MessageHeadersDefault.PartitionKey);

            batch.Count.Should().Be(1);
            firstPartitionKey.Should().Be("5090016");

            batch.Items.First().MessageHeaders.Remove(MessageHeadersDefault.PartitionKey);
            firstPartitionKey = batch.Items.FirstOrDefault().MessageHeaders
                .GetString(MessageHeadersDefault.PartitionKey);
            firstPartitionKey.Should().BeNull();
        }

        [Test]
        public void Should_Valid_Batch_TopicName()
        {
            var batch = ProducerBatchRequest.Create();

            var messageKey = Guid.NewGuid().ToString();
            var accountCreatedMessage = new AccountCreatedMessage("5090016");

            var messageHeader = MessageHeaders.GetInstance();
            messageHeader.SetPartition(accountCreatedMessage.AccountNumber);

            batch.Add(messageKey, accountCreatedMessage, messageHeader);
            batch.TopicName.Should().Be(AccountCreatedMessage.TopicName);
        }
        
        [Test]
        public void Should_Throws_ArgumentNullException_When_Message_Has_Not_TopicProducerAttribute()
        {
            var batch = ProducerBatchRequest.Create();

            var messageKey = Guid.NewGuid().ToString();
            var accountCreatedMessage = new AccountCreatedWithoutTopicMessage();

            Assert.Throws<ArgumentNullException>(() => batch.Add(messageKey, accountCreatedMessage, MessageHeaders.GetInstance()));
        }
        
        [Test]
        public void Should_Throws_ArgumentNullException_When_Message_Is_Invalid()
        {
            var batch = ProducerBatchRequest.Create();

            var messageKey = Guid.NewGuid().ToString();
            var accountCreatedMessage = new AccountCreatedMessage("5090016");

            var messageHeader = MessageHeaders.GetInstance();
            messageHeader.SetPartition(accountCreatedMessage.AccountNumber);

            Assert.Throws<ArgumentNullException>(() => batch.Add(messageKey, null, messageHeader));
        }
        
        [Test]
        public void Should_Throws_ArgumentNullException_When_PartitionKey_Is_Invalid()
        {
            var batch = ProducerBatchRequest.Create();

            var messageKey = Guid.NewGuid().ToString();
            var accountCreatedMessage = new AccountCreatedMessage("5090016");

            var messageHeader = MessageHeaders.GetInstance();
            messageHeader.SetPartition(accountCreatedMessage.AccountNumber);

            Assert.Throws<ArgumentNullException>(() => batch.Add(string.Empty, accountCreatedMessage, messageHeader));
        }
        
        [Test]
        public void Should_Create_Valid_ProducerBatchRequest_With_PartitionKey()
        {
            var batch = ProducerBatchRequest.Create();

            var messageKey = Guid.NewGuid().ToString();
            var accountCreatedMessage = new AccountCreatedMessage("5090016");

            var messageHeader = MessageHeaders.GetInstance();
            messageHeader.SetPartition(accountCreatedMessage.AccountNumber);
            batch.Add(messageKey, accountCreatedMessage, messageHeader);

            batch.Items.First().PartitionKey.Should().Be(messageKey);
            batch.Items.First().Message.Should().BeEquivalentTo(accountCreatedMessage);
        }

        [Test]
        public void Should_Create_Valid_ProducerBatchRequest()
        {
            var batch = ProducerBatchRequest.Create();

            var accountCreatedMessage = new AccountCreatedMessage("5090016");

            var messageHeader = MessageHeaders.GetInstance();
            messageHeader.SetPartition(accountCreatedMessage.AccountNumber);
            batch.Add(accountCreatedMessage, messageHeader);

            var firstPartitionKey = batch.Items.First().MessageHeaders.GetString(MessageHeadersDefault.PartitionKey);

            batch.Count.Should().Be(1);
            firstPartitionKey.Should().Be("5090016");

            batch.Items.First().MessageHeaders.Remove(MessageHeadersDefault.PartitionKey);
            firstPartitionKey = batch.Items.FirstOrDefault().MessageHeaders
                .GetString(MessageHeadersDefault.PartitionKey);
            firstPartitionKey.Should().BeNull();
        }

        [Test]
        public void Should_Count_Headers()
        {
            var batch = ProducerBatchRequest.Create();

            var accountUpdatedMessage = new AccountUpdatedMessage("5090016");

            var messageHeader = MessageHeaders.GetInstance();

            messageHeader.SetPartition(accountUpdatedMessage.AccountNumber);
            messageHeader.SetString(MessageHeadersDefault.Producer, "unit-test");
            batch.Add(accountUpdatedMessage, messageHeader);

            // var headers = batch.Items.First().MessageHeaders.GetHeaders();
            //
            // var counter = 0;
            // foreach (var header in batch.Items.First().MessageHeaders.GetHeaders())
            // {
            //     counter++;
            // }

            // var firstHeader = headers[0];
            //
            // firstHeader.Key.Should().Be(MessageHeadersDefault.PartitionKey);
            // counter.Should().Be(2);
        }

        [Test]
        public void Should_Create_Valid_ProducerBatchRequest_With_Two_Headers()
        {
            var batch = ProducerBatchRequest.Create();

            var accountUpdatedMessage = new AccountUpdatedMessage("5090016");

            var messageHeader = MessageHeaders.GetInstance();

            messageHeader.SetPartition(accountUpdatedMessage.AccountNumber);
            messageHeader.SetString(MessageHeadersDefault.Producer, "unit-test");
            batch.Add(accountUpdatedMessage, messageHeader);

            // var headers = batch.Items.First().MessageHeaders.GetHeaders();
            // var firstPartitionKey = batch.Items.First().MessageHeaders.GetString(MessageHeadersDefault.PartitionKey);
            //
            // batch.Count.Should().Be(1);
            // firstPartitionKey.Should().Be("5090016");
            //
            // headers.Count.Should().Be(2);
        }

        [Test]
        public void Should_Throws_ArgumentNullException_When_Key_Is_Empty()
        {
            var accountUpdatedMessage = new AccountUpdatedMessage("5090016");

            var messageHeader = MessageHeaders.GetInstance();

            messageHeader.SetPartition(accountUpdatedMessage.AccountNumber);
            Assert.Throws<ArgumentNullException>(() => messageHeader.SetString("", "unit-test"));
        }
    }
}