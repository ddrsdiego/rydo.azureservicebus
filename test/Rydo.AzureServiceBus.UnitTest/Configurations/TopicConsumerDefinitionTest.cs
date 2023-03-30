namespace Rydo.AzureServiceBus.UnitTest.Configurations
{
    using System;
    using Client.Configurations;
    using Fakes;
    using FluentAssertions;
    using NUnit.Framework;

    public class TopicConsumerDefinitionTest
    {
        [Test]
        public void Should_Create_Valid_TopicConsumerDefinition_With_Default_Values()
        {
            const string topicName = "topic-name";
            var subscriptionName = typeof(AccountCreatedMessage).Assembly.GetName().Name.ToLowerInvariant();

            var topicProducerDefinition = new TopicConsumerDefinition(topicName, subscriptionName);

            topicProducerDefinition.TopicName.Should().Be(topicName);
            topicProducerDefinition.SubscriptionName.Should().Be(subscriptionName);
            topicProducerDefinition.Direction.Should().Be(TopicDirections.Consumer);
            topicProducerDefinition.MaxDeliveryCount.Should().Be(TopicConsumerDefaultValues.MaxDeliveryCount);
            topicProducerDefinition.LockDurationInSeconds.Should()
                .Be(TimeSpan.FromSeconds(TopicConsumerDefaultValues.LockDurationInSeconds));
        }

        [Test]
        public void Should_Create_Valid_TopicConsumerDefinition()
        {
            const string topicName = "topic-name";
            var subscriptionName = typeof(AccountCreatedMessage).Assembly.GetName().Name.ToLowerInvariant();

            var topicProducerDefinition = new TopicConsumerDefinition(topicName, subscriptionName, 120, 5);

            topicProducerDefinition.TopicName.Should().Be(topicName);
            topicProducerDefinition.SubscriptionName.Should().Be(subscriptionName);
            topicProducerDefinition.Direction.Should().Be(TopicDirections.Consumer);
            topicProducerDefinition.MaxDeliveryCount.Should().Be(5);
            topicProducerDefinition.LockDurationInSeconds.Should().Be(TimeSpan.FromSeconds(120));
        }
    }
}