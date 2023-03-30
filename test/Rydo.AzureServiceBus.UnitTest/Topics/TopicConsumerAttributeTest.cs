namespace Rydo.AzureServiceBus.UnitTest.Topics
{
    using System;
    using Client.Topics;
    using FluentAssertions;
    using NUnit.Framework;

    public class TopicConsumerAttributeTest
    {
        [Test]
        public void Should_Create_Valid_TopicConsumerAttribute_With_TopicName()
        {
            const string topicName = "account-created";

            var topicProducerAttribute = new TopicConsumerAttribute(topicName);
            topicProducerAttribute.TopicName.Should().Be(topicName);
        }

        [Test]
        public void Should_Create_Valid_TopicConsumerAttribute_With_TopicName_And_ConsumerGroupId()
        {
            const string topicName = "account-created";
            var consumerGroupId = $"consumer-group-id-{Guid.NewGuid()}";

            var topicProducerAttribute = new TopicConsumerAttribute(topicName, consumerGroupId);
            
            topicProducerAttribute.TopicName.Should().Be(topicName);
            topicProducerAttribute.SubscriptionName.Should().Be(consumerGroupId);
        }
    }
}