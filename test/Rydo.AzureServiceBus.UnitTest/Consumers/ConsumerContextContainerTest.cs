namespace Rydo.AzureServiceBus.UnitTest.Consumers
{
    using System;
    using Client.Consumers.Subscribers;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    public class ConsumerContextContainerTest
    {
        [Test]
        public void Should_Trows_ArgumentNullException_When_TopicName_Is_Null()
        {
            var services = new ServiceCollection();
            var sut = new SubscriberContextContainer(services);

            Assert.Throws<ArgumentNullException>(() => sut.AddSubscriber(null));
        }
        
        [Test]
        public void Should_Trows_ArgumentNullException_When_TopicName_Is_Empty()
        {
            var services = new ServiceCollection();
            var sut = new SubscriberContextContainer(services);
            
            Assert.Throws<ArgumentNullException>(() => sut.AddSubscriber(string.Empty));
        }
        
        [Test]
        public void Test()
        {
            var services = new ServiceCollection();
            var sut = new SubscriberContextContainer(services);

            sut.AddSubscriber("rydo-azure-servicebus-account-created");
        }
    }
}