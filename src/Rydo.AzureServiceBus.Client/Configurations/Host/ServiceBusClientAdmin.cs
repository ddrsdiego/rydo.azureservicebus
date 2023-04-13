namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Abstractions.Observers.Observables;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;
    using Consumers.Subscribers;
    using Utils;

    public interface IServiceBusClientAdmin : IAdminObserverConnector, IAsyncDisposable
    {
        Task CreateEntitiesIfNotExistAsync(ISubscriberContext context,
            CancellationToken cancellationToken = default);
    }

    internal sealed class ServiceBusClientAdmin : IServiceBusClientAdmin
    {
        private readonly IServiceBusHostSettings _hostSettings;
        private readonly AdminClientClientObservable _adminClientClientObservable;

        internal ServiceBusClientAdmin(IServiceBusHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
            _adminClientClientObservable = new AdminClientClientObservable();
        }

        public ValueTask DisposeAsync() => new ValueTask(Task.CompletedTask);

        public async Task CreateEntitiesIfNotExistAsync(ISubscriberContext context,
            CancellationToken cancellationToken = default)
        {
            var subscriberContext = (SubscriberContext) context;
            
            await CreateQueueIfNotExistAsync(subscriberContext, cancellationToken);
            await CreateTopicIfNotExistAsync(subscriberContext, cancellationToken);
            await CreateSubscriptionIfNotExistAsync(subscriberContext, cancellationToken);
        }

        public IConnectHandle ConnectAdminClientObservers(IAdminClientObserver clientObserver) =>
            _adminClientClientObservable.Connect(clientObserver);

        private async Task CreateSubscriptionIfNotExistAsync(SubscriberContext subscriberContext,
            CancellationToken cancellationToken)
        {
            var subscriptionOptions = new CreateSubscriptionOptions(subscriberContext.Specification.TopicName,
                subscriberContext.Specification.SubscriptionName)
            {
                ForwardTo = subscriberContext.Specification.QueueName,
                LockDuration = subscriberContext.Specification.LockDurationInSeconds,
                MaxDeliveryCount = subscriberContext.Specification.MaxDeliveryCount,
            };
            
            try
            {
                await _adminClientClientObservable.PreConsumerAsync(subscriberContext);

                var subscriptionExists =
                    await _hostSettings.AdminClient.SubscriptionExistsAsync(subscriberContext.Specification.TopicName,
                        subscriberContext.Specification.SubscriptionName,
                        cancellationToken);

                if (!subscriptionExists.Value)
                    await _hostSettings.AdminClient.CreateSubscriptionAsync(subscriptionOptions, cancellationToken);
            }
            catch (ServiceBusException)
            {
                await _hostSettings.AdminClient.CreateSubscriptionAsync(subscriptionOptions, cancellationToken);
            }
        }

        private async Task CreateTopicIfNotExistAsync(SubscriberContext context, CancellationToken cancellationToken)
        {
            var topicExists =
                await _hostSettings.AdminClient.TopicExistsAsync(context.Specification.TopicName, cancellationToken);

            var topicOptions = new CreateTopicOptions(context.Specification.TopicName);
            
            try
            {
                if (!topicExists.Value)
                    await _hostSettings.AdminClient.CreateTopicAsync(topicOptions, cancellationToken);
            }
            catch (ServiceBusException)
            {
                // most likely a race between two clients trying to create the same topic - we should be able to get it now
                if (!topicExists.Value)
                    await _hostSettings.AdminClient.CreateTopicAsync(topicOptions, cancellationToken);
            }
        }

        private async Task CreateQueueIfNotExistAsync(SubscriberContext subscriberContext,
            CancellationToken cancellationToken)
        {
            var queueOptions = new CreateQueueOptions(subscriberContext.Specification.QueueName)
            {
                LockDuration = subscriberContext.Specification.LockDurationInSeconds,
                MaxDeliveryCount = subscriberContext.Specification.MaxDeliveryCount,
                AutoDeleteOnIdle = subscriberContext.Specification.AutoDeleteOnIdle
            };

            try
            {
                await _adminClientClientObservable.VerifyQueueExitsAsync(queueOptions);

                var queueExists =
                    await _hostSettings.AdminClient.QueueExistsAsync(subscriberContext.Specification.QueueName,
                        cancellationToken);

                if (!queueExists.Value)
                    await _hostSettings.AdminClient.CreateQueueAsync(queueOptions, cancellationToken);

                await _adminClientClientObservable.PreConsumerAsync(subscriberContext);
            }
            catch (ServiceBusException)
            {
                // most likely a race between two clients trying to create the same queue - we should be able to get it now
                await _hostSettings.AdminClient.CreateQueueAsync(queueOptions, cancellationToken);
            }
        }
    }
}