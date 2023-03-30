namespace Rydo.AzureServiceBus.UnitTest.Producers
{
    using System;
    using Client.Headers;
    using Client.Producers;
    using Fakes;
    using FluentAssertions;
    using NUnit.Framework;

    public class ProducerRequestTest
    {
        [Test]
        public void Should_Create_Valid_ProducerRequest()
        {
            const string topicName = "account-created";
            var message = new AccountCreatedMessage("5090016");

            var headers = MessageHeaders.GetInstance();
            var request = ProducerRequest.GetInstance(topicName, message, headers);

            request.TopicName.Should().Be(topicName);
            request.Message.Should().BeOfType<AccountCreatedMessage>();
        }

        [Test]
        public void Should_Create_Valid_ProducerRequest_With_PartitionKey()
        {
            const string topicName = "account-created";
            var partitionKey = Guid.NewGuid().ToString();
            var message = new AccountCreatedMessage("5090016");

            var headers = MessageHeaders.GetInstance();
            var request = ProducerRequest.GetInstance(topicName, partitionKey, message, headers);

            request.PartitionKey.Should().Be(partitionKey);
            request.TopicName.Should().Be(topicName);
            request.Message.Should().BeOfType<AccountCreatedMessage>();
        }
    }
}