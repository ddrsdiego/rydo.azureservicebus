namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Abstractions.Observers.Observables;
    using Azure.Messaging.ServiceBus.Administration;
    using Consumers.Subscribers;
    using Utils;

    public interface IServiceBusClientAdmin : IAdminObserverConnector, IAsyncDisposable
    {
        Task CreateEntitiesIfNotExistAsync(SubscriberContext subscriberContext,
            CancellationToken cancellationToken = default);
    }

    internal sealed class ServiceBusClientAdmin : IServiceBusClientAdmin
    {
        private readonly IServiceBusHostSettings _hostSettings;
        private readonly AdminClientClientObservable _adminClientClientObservable;

        public ServiceBusClientAdmin(IServiceBusHostSettings hostSettings)
        {
            _hostSettings = hostSettings;
            _adminClientClientObservable = new AdminClientClientObservable();
        }

        public ValueTask DisposeAsync() => new ValueTask(Task.CompletedTask);

        public async Task CreateEntitiesIfNotExistAsync(SubscriberContext subscriberContext,
            CancellationToken cancellationToken = default)
        {
            await CreateQueueIfNotExistAsync(subscriberContext, cancellationToken);
            await CreateTopicIfNotExistAsync(subscriberContext, cancellationToken);
            await CreateSubscriptionIfNotExistAsync(subscriberContext, cancellationToken);
        }

        public IConnectHandle ConnectAdminClientObservers(IAdminClientObserver clientObserver) =>
            _adminClientClientObservable.Connect(clientObserver);

        private async Task CreateSubscriptionIfNotExistAsync(SubscriberContext subscriberContext,
            CancellationToken cancellationToken)
        {
            try
            {
                await _adminClientClientObservable.PreConsumerAsync(subscriberContext);
                

                var subscriptionOptions = new CreateSubscriptionOptions(subscriberContext.Specification.TopicName,
                    subscriberContext.Specification.SubscriptionName)
                {
                    ForwardTo = subscriberContext.TopicSubscriptionName,
                    LockDuration = subscriberContext.Specification.LockDurationInSeconds,
                    MaxDeliveryCount = subscriberContext.Specification.MaxDeliveryCount,
                };

                var subscriptionExists =
                    await _hostSettings.AdminClient.SubscriptionExistsAsync(subscriberContext.Specification.TopicName,
                        subscriberContext.Specification.SubscriptionName,
                        cancellationToken);

                if (!subscriptionExists.Value)
                    await _hostSettings.AdminClient.CreateSubscriptionAsync(subscriptionOptions, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task CreateTopicIfNotExistAsync(SubscriberContext context, CancellationToken cancellationToken)
        {
            var topicExists =
                await _hostSettings.AdminClient.TopicExistsAsync(context.Specification.TopicName, cancellationToken);

            var topicOptions = new CreateTopicOptions(context.Specification.TopicName);
            if (!topicExists.Value)
                await _hostSettings.AdminClient.CreateTopicAsync(topicOptions, cancellationToken);
        }

        private async Task CreateQueueIfNotExistAsync(SubscriberContext subscriberContext,
            CancellationToken cancellationToken)
        {
            try
            {
                var queueOptions = new CreateQueueOptions(subscriberContext.TopicSubscriptionName)
                {
                    LockDuration = subscriberContext.Specification.LockDurationInSeconds,
                    MaxDeliveryCount = subscriberContext.Specification.MaxDeliveryCount,
                    AutoDeleteOnIdle = subscriberContext.Specification.AutoDeleteOnIdle
                };

                await _adminClientClientObservable.VerifyQueueExitsAsync(queueOptions);

                var queueExists =
                    await _hostSettings.AdminClient.QueueExistsAsync(subscriberContext.TopicSubscriptionName,
                        cancellationToken);

                if (!queueExists.Value)
                    await _hostSettings.AdminClient.CreateQueueAsync(queueOptions, cancellationToken);

                await _adminClientClientObservable.PreConsumerAsync(subscriberContext);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}