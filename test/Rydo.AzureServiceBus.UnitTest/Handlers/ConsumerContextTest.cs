namespace Rydo.AzureServiceBus.UnitTest.Handlers
{
    using Client.Consumers.Subscribers;
    using NUnit.Framework;

    internal sealed class ConsumerSpecificationBuilderTest
    {
        private string _topicName;
        private int _maxDelivery;
        private string _subscriptionName;
        private int _maxDeliveryCount;
        private int _lockDurationInMinutes;
        
        public ConsumerSpecificationBuilderTest LockDurationInMinutes(int lockDurationInMinutes)
        {
            _lockDurationInMinutes = lockDurationInMinutes;
            return this;
        }
        
        public ConsumerSpecificationBuilderTest MaxDeliveryCount(int maxDeliveryCount)
        {
            _maxDeliveryCount = maxDeliveryCount;
            return this;
        }
        
        public ConsumerSpecificationBuilderTest MaxDelivery(int maxDelivery)
        {
            _maxDelivery = maxDelivery;
            return this;
        }

        public ConsumerSpecificationBuilderTest TopicName(string topicName)
        {
            _topicName = _topicName;
            return this;
        }

        public ConsumerSpecificationBuilderTest SubscriptionName(string subscriptionName)
        {
            _subscriptionName = subscriptionName;
            return this;
        }

        public SubscriberSpecification Build()
        {
            var builder = new SubscriberConfiguratorBuilder(_topicName, _subscriptionName);
            var consumerConfigurator = builder
                .MaxMessages(10)
                .MaxDeliveryCount(1000)
                .LockDurationInMinutes(1)
                .Build();
            
            return new SubscriberSpecification(consumerConfigurator.Value);
        }
    }
    //
    // public class ConsumerContextTest
    // {
    //     [Test]
    //     public void Test()
    //     {
    //         var consumerSpecification = new ConsumerSpecificationBuilderTest()
    //             .TopicName("account-created")
    //             .SubscriptionName("Rydo.AzureServiceBus.UnitTest.Handlers")
    //             .MaxDelivery(10)
    //             .MaxDeliveryCount(1000)
    //             .LockDurationInMinutes(1)
    //             .Build();
    //     }
    // }
}