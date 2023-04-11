namespace Rydo.AzureServiceBus.UnitTest.Subscribers
{
    using Azure.Messaging.ServiceBus;
    using Client.Consumers.Subscribers;
    using NSubstitute;
    using NUnit.Framework;

    public class ServiceBusMessageContextTest
    {
        [Test]
        public void Test()
        {
            var serviceBusReceivedMessage = Substitute.For<ServiceBusReceivedMessage>();
            var context = new ServiceBusMessageContext(serviceBusReceivedMessage);
        }
    }
}