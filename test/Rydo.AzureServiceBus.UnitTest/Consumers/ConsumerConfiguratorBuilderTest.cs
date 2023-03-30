namespace Rydo.AzureServiceBus.UnitTest.Consumers
{
    using Client.Configurations;
    using Client.Consumers;
    using FluentAssertions;
    using NUnit.Framework;

    public class ConsumerConfiguratorBuilderTest
    {
        [Test]
        public void Should_Create_ConsumerConfigurator_When_BufferSize_Equals_Zero()
        {
            var builder = new ConsumerConfiguratorBuilder("topic-name");
            var consumerConfigurator = builder
                .SetBufferSize(0)
                .Build();

            consumerConfigurator.BufferSize.Should().Be(TopicConsumerDefaultValues.BufferSize);
            consumerConfigurator.MaxMessages.Should().Be(TopicConsumerDefaultValues.MaxMessages);
            consumerConfigurator.MaxDeliveryCount.Should().Be(TopicConsumerDefaultValues.MaxDeliveryCount);
        }

        [Test]
        public void Should_Create_ConsumerConfigurator_With_Invalid_MaxDeliveryCount()
        {
            var builder = new ConsumerConfiguratorBuilder("topic-name");
            const string subscriptionName = "rydo.azure.servicebus.unittest.consumers";
            
            var consumerConfigurator = builder
                .SetBufferSize(1_000)
                .SetMaxDeliveryCount(-100)
                .SetMaxMessages(2000)
                .SetLockDurationInMinutes(5)
                .SetSubscriptionName(subscriptionName)
                .Build();

            consumerConfigurator.BufferSize.Should().Be(1_000);
            consumerConfigurator.MaxDeliveryCount.Should().Be(10);
            consumerConfigurator.LockDurationInMinutes.Should().Be(5);
            consumerConfigurator.MaxMessages.Should().Be(2000);
            consumerConfigurator.SubscriptionName.Should().Be(subscriptionName);
            consumerConfigurator.MaxDeliveryCount.Should().Be(TopicConsumerDefaultValues.MaxDeliveryCount);
        }
        
        [Test]
        public void Should_Create_ConsumerConfigurator_With_Users_Valid()
        {
            var builder = new ConsumerConfiguratorBuilder("topic-name");
            const string subscriptionName = "rydo.azure.servicebus.unittest.consumers";
            
            var consumerConfigurator = builder
                .SetBufferSize(1_000)
                .SetMaxDeliveryCount(10)
                .SetMaxMessages(2000)
                .SetLockDurationInMinutes(5)
                .SetSubscriptionName(subscriptionName)
                .Build();

            consumerConfigurator.BufferSize.Should().Be(1_000);
            consumerConfigurator.MaxDeliveryCount.Should().Be(10);
            consumerConfigurator.LockDurationInMinutes.Should().Be(5);
            consumerConfigurator.MaxMessages.Should().Be(2000);
            consumerConfigurator.SubscriptionName.Should().Be(subscriptionName);
            consumerConfigurator.MaxDeliveryCount.Should().Be(TopicConsumerDefaultValues.MaxDeliveryCount);
        }
        
        [Test]
        public void Should_Create_ConsumerConfigurator_With_BufferSize()
        {
            var builder = new ConsumerConfiguratorBuilder("topic-name");
            var consumerConfigurator = builder
                .SetBufferSize(1_000)
                .Build();

            consumerConfigurator.BufferSize.Should().Be(1_000);
            consumerConfigurator.MaxMessages.Should().Be(TopicConsumerDefaultValues.MaxMessages);
            consumerConfigurator.MaxDeliveryCount.Should().Be(TopicConsumerDefaultValues.MaxDeliveryCount);
        }
        
        [Test]
        public void Should_Create_ConsumerConfigurator_With_Default_Values()
        {
            var builder = new ConsumerConfiguratorBuilder("topic-name");
            var consumerConfigurator = builder.Build();

            consumerConfigurator.BufferSize.Should().Be(TopicConsumerDefaultValues.BufferSize);
            consumerConfigurator.MaxDeliveryCount.Should().Be(TopicConsumerDefaultValues.MaxDeliveryCount);
            consumerConfigurator.MaxMessages.Should().Be(TopicConsumerDefaultValues.MaxMessages);
        }
    }
}