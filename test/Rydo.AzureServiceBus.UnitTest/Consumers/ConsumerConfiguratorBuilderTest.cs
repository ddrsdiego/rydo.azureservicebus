namespace Rydo.AzureServiceBus.UnitTest.Consumers
{
    using Client.Configurations;
    using Client.Consumers.Subscribers;
    using FluentAssertions;
    using NUnit.Framework;

    public class ConsumerConfiguratorBuilderTest
    {
        [Test]
        public void Should_Create_ConsumerConfigurator_When_BufferSize_Equals_Zero()
        {
            var builder = new SubscriberConfiguratorBuilder("topic-name");
            var consumerConfigurator = builder
                .BufferSize(0)
                .Build();

            consumerConfigurator.Value.BufferSize.Should().Be(TopicConsumerDefaultValues.BufferSize);
            consumerConfigurator.Value.MaxMessages.Should().Be(TopicConsumerDefaultValues.MaxMessages);
            consumerConfigurator.Value.MaxDeliveryCount.Should().Be(TopicConsumerDefaultValues.MaxDeliveryCount);
        }

        [Test]
        public void Should_Create_ConsumerConfigurator_With_Invalid_MaxDeliveryCount()
        {
            var builder = new SubscriberConfiguratorBuilder("topic-name");
            const string subscriptionName = "rydo.azure.servicebus.unittest.consumers";
            
            var consumerConfigurator = builder
                .BufferSize(1_000)
                .MaxDeliveryCount(-100)
                .MaxMessages(2000)
                .LockDurationInMinutes(5)
                .SubscriptionName(subscriptionName)
                .Build();

            consumerConfigurator.Value.BufferSize.Should().Be(1_000);
            consumerConfigurator.Value.MaxDeliveryCount.Should().Be(10);
            consumerConfigurator.Value.LockDurationInMinutes.Should().Be(5);
            consumerConfigurator.Value.MaxMessages.Should().Be(2000);
            consumerConfigurator.Value.SubscriptionName.Should().Be(subscriptionName);
            consumerConfigurator.Value.MaxDeliveryCount.Should().Be(TopicConsumerDefaultValues.MaxDeliveryCount);
        }
        
        [Test]
        public void Should_Create_ConsumerConfigurator_With_Users_Valid()
        {
            var builder = new SubscriberConfiguratorBuilder("topic-name");
            const string subscriptionName = "rydo.azure.servicebus.unittest.consumers";
            
            var consumerConfigurator = builder
                .BufferSize(1_000)
                .MaxDeliveryCount(10)
                .MaxMessages(2000)
                .LockDurationInMinutes(5)
                .SubscriptionName(subscriptionName)
                .Build();

            consumerConfigurator.Value.BufferSize.Should().Be(1_000);
            consumerConfigurator.Value.MaxDeliveryCount.Should().Be(10);
            consumerConfigurator.Value.LockDurationInMinutes.Should().Be(5);
            consumerConfigurator.Value.MaxMessages.Should().Be(2000);
            consumerConfigurator.Value.SubscriptionName.Should().Be(subscriptionName);
            consumerConfigurator.Value.MaxDeliveryCount.Should().Be(TopicConsumerDefaultValues.MaxDeliveryCount);
        }
        
        [Test]
        public void Should_Create_ConsumerConfigurator_With_BufferSize()
        {
            var builder = new SubscriberConfiguratorBuilder("topic-name");
            var consumerConfigurator = builder
                .BufferSize(1_000)
                .Build();

            consumerConfigurator.Value.BufferSize.Should().Be(1_000);
            consumerConfigurator.Value.MaxMessages.Should().Be(TopicConsumerDefaultValues.MaxMessages);
            consumerConfigurator.Value.MaxDeliveryCount.Should().Be(TopicConsumerDefaultValues.MaxDeliveryCount);
        }
        
        [Test]
        public void Should_Create_ConsumerConfigurator_With_Default_Values()
        {
            var builder = new SubscriberConfiguratorBuilder("topic-name");
            var consumerConfigurator = builder.Build();

            consumerConfigurator.Value.BufferSize.Should().Be(TopicConsumerDefaultValues.BufferSize);
            consumerConfigurator.Value.MaxDeliveryCount.Should().Be(TopicConsumerDefaultValues.MaxDeliveryCount);
            consumerConfigurator.Value.MaxMessages.Should().Be(TopicConsumerDefaultValues.MaxMessages);
        }
    }
}