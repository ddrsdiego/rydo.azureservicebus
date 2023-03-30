namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using CSharpFunctionalExtensions;
    using Configurations;

    public sealed class SubscriberConfiguratorBuilder
    {
        private readonly string _topicName;
        private int _maxMessages;
        private string _subscriptionName;
        private int _lockDurationInMinutes;
        private int _bufferSize;
        private int _maxDeliveryCount;
        private bool _neverAutoDelete;
        private int _autoDeleteAfterIdleInHours;

        internal SubscriberConfiguratorBuilder(string topicName)
        {
            HasBuild = false;
            
            _topicName = topicName;
            _subscriptionName = string.Empty;
            _autoDeleteAfterIdleInHours = TopicConsumerDefaultValues.AutoDeleteAfterIdleInHours;
            _neverAutoDelete = TopicConsumerDefaultValues.NeverAutoDelete;
            _maxDeliveryCount = TopicConsumerDefaultValues.MaxDeliveryCount;
            _bufferSize = TopicConsumerDefaultValues.BufferSize;
            _maxMessages = TopicConsumerDefaultValues.MaxMessages;
            _lockDurationInMinutes = TopicConsumerDefaultValues.LockDurationInSeconds;
        }

        public bool HasBuild { get; private set; }

        /// <summary>
        /// Subscription names can contain letters, numbers, periods (.), hyphens (-), and underscores (_), up to 50 characters. Subscription names are also case-insensitive.
        /// </summary>
        /// <param name="subscriptionName"></param>
        /// <returns></returns>
        public SubscriberConfiguratorBuilder SubscriptionName(string subscriptionName)
        {
            _subscriptionName = subscriptionName;
            return this;
        }

        public SubscriberConfiguratorBuilder AutoDeleteAfterIdleInHours(int autoDeleteAfterIdleInHours)
        {
            _autoDeleteAfterIdleInHours = autoDeleteAfterIdleInHours;
            return this;
        }
        
        public SubscriberConfiguratorBuilder NeverAutoDelete(bool neverAutoDelete)
        {
            _neverAutoDelete = neverAutoDelete;
            return this;
        }

        /// <summary>
        /// Sets the amount of time that a message is locked for other receivers.
        /// After its lock expires, a message pulled by one receiver becomes available to be pulled by other receivers.
        /// Defaults to 30 seconds, with a maximum of 5 minutes.
        /// </summary>
        /// <param name="lockDurationInMinutes"></param>
        /// <returns></returns>
        public SubscriberConfiguratorBuilder LockDurationInMinutes(int lockDurationInMinutes)
        {
            _lockDurationInMinutes = lockDurationInMinutes;
            return this;
        }

        /// <summary>
        /// Number of maximum deliveries, value ranging from 1 to 2000.
        /// </summary>
        /// <param name="maxDeliveryCount"></param>
        /// <returns></returns>
        public SubscriberConfiguratorBuilder MaxDeliveryCount(int maxDeliveryCount)
        {
            if (maxDeliveryCount <= 0)
                maxDeliveryCount = TopicConsumerDefaultValues.MaxDeliveryCount;

            _maxDeliveryCount = maxDeliveryCount;
            return this;
        }

        public SubscriberConfiguratorBuilder MaxMessages(int maxMessages)
        {
            _maxMessages = maxMessages;
            return this;
        }

        public SubscriberConfiguratorBuilder BufferSize(int bufferSize)
        {
            if (bufferSize <= 0)
                bufferSize = TopicConsumerDefaultValues.BufferSize;

            _bufferSize = bufferSize;
            return this;
        }

        internal IConsumerConfigurator ConsumerConfigurator { get; private set; }

        internal Result<IConsumerConfigurator> Build()
        {
            ConsumerConfigurator = new ConsumerConfigurator(_topicName, _subscriptionName)
            {
                LockDurationInMinutes = _lockDurationInMinutes,
                MaxDeliveryCount = _maxDeliveryCount,
                MaxMessages = _maxMessages,
                BufferSize = _bufferSize,
                NeverAutoDelete = _neverAutoDelete,
                AutoDeleteAfterIdleInHours = _autoDeleteAfterIdleInHours
            };

            HasBuild = true;
            return Result.Success(ConsumerConfigurator);
        }
    }
}