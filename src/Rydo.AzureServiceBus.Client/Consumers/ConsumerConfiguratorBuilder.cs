namespace Rydo.AzureServiceBus.Client.Consumers
{
    using Configurations;

    public sealed class ConsumerConfiguratorBuilder
    {
        private readonly string _topicName;
        private int _maxMessages;
        private string _subscriptionName;
        private int _lockDurationInMinutes;
        private int _bufferSize;
        private int _maxDeliveryCount;

        internal ConsumerConfiguratorBuilder(string topicName)
        {
            _topicName = topicName;
            HasBuild = false;
            _subscriptionName = string.Empty;
            _maxDeliveryCount = TopicConsumerDefaultValues.MaxDeliveryCount;
            _bufferSize = TopicConsumerDefaultValues.BufferSize;
            _maxMessages = TopicConsumerDefaultValues.MaxMessages;
            _lockDurationInMinutes = TopicConsumerDefaultValues.LockDurationInSeconds;
        }

        public bool HasBuild { get; private set; }

        public ConsumerConfiguratorBuilder SetSubscriptionName(string subscriptionName)
        {
            _subscriptionName = subscriptionName;
            return this;
        }

        public ConsumerConfiguratorBuilder SetLockDurationInMinutes(int lockDurationInMinutes)
        {
            _lockDurationInMinutes = lockDurationInMinutes;
            return this;
        }

        public ConsumerConfiguratorBuilder SetMaxDeliveryCount(int maxDeliveryCount)
        {
            if (maxDeliveryCount <= 0)
                maxDeliveryCount = TopicConsumerDefaultValues.MaxDeliveryCount;

            _maxDeliveryCount = maxDeliveryCount;
            return this;
        }

        public ConsumerConfiguratorBuilder SetMaxMessages(int maxMessages)
        {
            _maxMessages = maxMessages;
            return this;
        }

        public ConsumerConfiguratorBuilder SetBufferSize(int bufferSize)
        {
            if (bufferSize <= 0)
                bufferSize = TopicConsumerDefaultValues.BufferSize;

            _bufferSize = bufferSize;
            return this;
        }

        internal IConsumerConfigurator ConsumerConfigurator { get; private set; }

        internal IConsumerConfigurator Build()
        {
            ConsumerConfigurator = new ConsumerConfigurator(_topicName, _subscriptionName)
            {
                LockDurationInMinutes = _lockDurationInMinutes,
                MaxDeliveryCount = _maxDeliveryCount,
                MaxMessages = _maxMessages,
                BufferSize = _bufferSize
            };

            HasBuild = true;
            return ConsumerConfigurator;
        }
    }
}