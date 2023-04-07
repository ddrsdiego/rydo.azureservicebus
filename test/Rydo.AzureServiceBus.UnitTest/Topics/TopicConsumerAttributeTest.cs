namespace Rydo.AzureServiceBus.UnitTest.Topics
{
    using Client.Topics;
    using Consumer.ConsumerHandlers;
    using FluentAssertions;
    using NUnit.Framework;

    public class TopicConsumerAttributeTest
    {
        [Test]
        public void Should_Create_Valid_TopicConsumerAttribute_With_TopicName()
        {
            const string topicName = "account-created";

            var topicProducerAttribute = new TopicConsumerAttribute(typeof(AccountCreated), topicName);
            topicProducerAttribute.TopicName.Should().Be(topicName);
            topicProducerAttribute.ContractType.Should().Be(typeof(AccountCreated));
        }
    }
}