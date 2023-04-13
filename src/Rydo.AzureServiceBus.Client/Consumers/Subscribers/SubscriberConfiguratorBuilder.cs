namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using Azure.Messaging.ServiceBus;
    using CSharpFunctionalExtensions;
    using Configurations;

    public sealed class SubscriberConfiguratorBuilder
    {
        private readonly string _topicName;
        private readonly string _subscriptionName;

        private int _prefetchCount;
        private int _maxMessages;
        private int _lockDurationInMinutes;
        private int _bufferSize;
        private int _maxDeliveryCount;
        private bool _neverAutoDelete;
        private int _autoDeleteAfterIdleInHours;
        private ServiceBusReceiveMode _receiveMode;

        internal SubscriberConfiguratorBuilder(string topicName, string subscriptionName)
        {
            _topicName = topicName ?? throw new ArgumentNullException(nameof(topicName));
            _subscriptionName = subscriptionName ?? throw new ArgumentNullException(nameof(subscriptionName));

            HasBuild = false;

            _receiveMode = ServiceBusReceiveMode.PeekLock;
            _bufferSize = TopicConsumerDefaultValues.BufferSize;
            _maxMessages = TopicConsumerDefaultValues.MaxMessages;
            _prefetchCount = TopicConsumerDefaultValues.PrefetchCount;
            _neverAutoDelete = TopicConsumerDefaultValues.NeverAutoDelete;
            _maxDeliveryCount = TopicConsumerDefaultValues.MaxDeliveryCount;
            _lockDurationInMinutes = TopicConsumerDefaultValues.LockDurationInSeconds;
            _autoDeleteAfterIdleInHours = TopicConsumerDefaultValues.AutoDeleteAfterIdleInHours;
        }

        public bool HasBuild { get; private set; }

        public SubscriberConfiguratorBuilder PrefetchCount(int prefetchCount)
        {
            if (prefetchCount <= 0)
                prefetchCount = TopicConsumerDefaultValues.PrefetchCount;

            _prefetchCount = prefetchCount;
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

        /// <summary>
        /// The maximum number of messages that will be received.
        /// </summary>
        /// <param name="maxMessages">The maximum number of messages that will be received. Default is 1</param>
        /// <returns></returns>
        public SubscriberConfiguratorBuilder MaxMessages(int maxMessages)
        {
            if (maxMessages <= 0)
                maxMessages = TopicConsumerDefaultValues.MaxMessages;

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

        private SubscriberConfiguratorBuilder ReceiveMode(ServiceBusReceiveMode receiveMode)
        {
            _receiveMode = receiveMode;
            return this;
        }

        internal IConsumerConfigurator ConsumerConfigurator { get; private set; }

        internal Result<IConsumerConfigurator> Build()
        {
            if (_prefetchCount <= _maxMessages)
                _prefetchCount = _maxMessages;

            ConsumerConfigurator = new ConsumerConfigurator(_topicName, _subscriptionName)
            {
                ReceiveMode = _receiveMode,
                PrefetchCount = _prefetchCount,
                LockDurationInSeconds = _lockDurationInMinutes,
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