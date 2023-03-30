namespace Rydo.AzureServiceBus.UnitTest.Producers
{
    using Client.Exceptions;
    using Client.Producers;
    using Fakes;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NUnit.Framework;

    public class TopicProducerManagerTest
    {
        private readonly ILogger<TopicProducerManager> _logger;

        public TopicProducerManagerTest()
        {
            _logger = Substitute.For<ILogger<TopicProducerManager>>();
        }

        [Test]
        public void Should_Failed_ExtractTopicName_From_Model_2()
        {
            var sut = new TopicProducerManager(_logger);
            var accountCreatedMessage = new AccountCreatedWithoutTopicMessage();
            var res = sut.TryExtractTopicName(accountCreatedMessage, out var topicName);
        }

        [Test]
        public void Should_Failed_ExtractTopicName_From_Model_1()
        {
            var sut = new TopicProducerManager(_logger);
            var accountCreatedMessage = new AccountCreatedInvalidMessage();
            Assert.Throws<InvalidTopicNameException>(() =>
                sut.TryExtractTopicName(accountCreatedMessage, out var topicName));
        }

        [Test]
        public void Should_Failed_ExtractTopicName_From_Model()
        {
            var sut = new TopicProducerManager(_logger);
            var res = sut.TryExtractTopicName(null, out var topicName);

            res.Should().BeFalse();
            topicName.Should().BeEmpty();
        }

        [Test]
        public void Should_ExtractTopicName_From_Model()
        {
            var sut = new TopicProducerManager(_logger);

            var accountCreatedMessage = new AccountCreatedMessage("5090016");
            var res = sut.TryExtractTopicName(accountCreatedMessage, out var topicName);

            res.Should().BeTrue();
            AccountCreatedMessage.TopicName.Should().Be(topicName);
        }

        [Test]
        public void Should_ExtractTopicName_From_Model_1()
        {
            var sut = new TopicProducerManager(_logger);

            var accountCreatedMessage = new AccountCreatedMessage("5090016");
            var res = sut.TryExtractTopicName(accountCreatedMessage, out var topicName);

            res = sut.TryExtractTopicName(accountCreatedMessage, out topicName);
            res.Should().BeTrue();
            AccountCreatedMessage.TopicName.Should().Be(topicName);
        }
    }
}