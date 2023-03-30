namespace Rydo.AzureServiceBus.UnitTest.Topics
{
    using Client.Topics;
    using FluentAssertions;
    using NUnit.Framework;

    public class TopicProducerAttributeTest
    {
        [Test]
        public void Should_Create_Valid_TopicProducerAttributeTest_With_TopicName()
        {
            const string topicName = "account-created";

            var topicProducerAttribute = new TopicProducerAttribute(topicName);
            topicProducerAttribute.TopicName.Should().Be(topicName);
        }
    }
}