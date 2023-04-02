namespace Rydo.AzureServiceBus.UnitTest.Receivers
{
    using Client.Configurations;
    using Client.Configurations.Receivers;
    using Client.Consumers.Subscribers;
    using Consumer.ConsumerHandlers;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    public class ReceiverContextContainerTest
    {
        [Test]
        public void Should_Instantiate_ReceiverContextContainer()
        {
            var services = new ServiceCollection();
            var container = new ReceiverContextContainer(services);

            container.Should().NotBeNull();
            container.Subscriber.Contexts.Count.Should().Be(0);
        }

        [Test]
        public void Should_Add_Consumer_Handler_When_Is_Default_Subscribers_Values()
        {
            var services = new ServiceCollection();
            var sut = new AzureServiceBusClientConfigurator(services);

            services.AddLogging();

            sut.Receiver.Configure<AccountUpdatedConsumerHandler>();
            sut.Receiver.Configure<AccountCreatedConsumerHandler>();

            var provider = services.BuildServiceProvider();
            var receiverListenerContainer = provider.GetRequiredService<IReceiverListenerContainer>();
            var subscriberContextContainer = provider.GetRequiredService<ISubscriberContextContainer>();
            
            receiverListenerContainer.Listeners.Count.Should().Be(2);
            subscriberContextContainer.Contexts.Count.Should().Be(2);
        }
    }
}