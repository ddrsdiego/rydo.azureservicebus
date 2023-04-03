namespace Rydo.AzureServiceBus.UnitTest.Consumers
{
    using Client.Producers;
    using Consumer;
    using Consumer.ConsumerHandlers;
    using FluentAssertions;
    using NUnit.Framework;

    public class ModelExtensionsTest
    {
        [Test]
        public void Should_Extract_TopicOrQueueName_From_ConsumerHandler()
        {
            var res = typeof(AccountCreatedConsumerHandler).TryExtractTopicNameFromConsumer(out var topicOrQueueName);
            
            res.Should().BeTrue();
            topicOrQueueName.Should().Be(TopicNameConstants.AccountCreated);
        }
    }
}