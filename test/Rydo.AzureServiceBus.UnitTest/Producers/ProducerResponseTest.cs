namespace Rydo.AzureServiceBus.UnitTest.Producers
{
    using Client.Headers;
    using Client.Producers;
    using Fakes;
    using FluentAssertions;
    using NUnit.Framework;

    public class ProducerResponseTest
    {
        [Test]
        public void Should_Create_ProducerResponse_With_Associate_Request()
        {
            const string topicName = "topic-name";

            var headers = MessageHeaders.GetInstance();
            var message = new AccountCreatedMessage("5090016");
            var request = ProducerRequest.GetInstance(topicName, message, headers);

            var sut = new ProducerResponse(request);

            sut.ResponseAt.Should().BeAfter(request.RequestedAt);
            sut.Request.PartitionKey.Should().Be(request.PartitionKey);
        }
    }
}