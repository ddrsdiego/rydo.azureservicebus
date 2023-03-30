namespace Rydo.AzureServiceBus.UnitTest.Configurations
{
    using Client.Configurations;
    using FluentAssertions;
    using NUnit.Framework;

    public class TopicProducerDefinitionTest
    {
        [Test]
        public void Should_Create_Valid_TopicProducerDefinition()
        {
            const string topicName = "topic-name";

            var topicProducerDefinition = new TopicProducerDefinition(topicName);

            topicProducerDefinition.TopicName.Should().Be(topicName);
            topicProducerDefinition.SubscriptionName.Should().BeEmpty();
            topicProducerDefinition.Direction.Should().Be(TopicDirections.Producer);
        }
    }
}