namespace Rydo.AzureServiceBus.Client.Abstractions.Observers.Observables
{
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus.Administration;
    using Consumers.Subscribers;
    using Utils;

    internal class AdminClientClientObservable : Connectable<IAdminClientObserver>, IAdminClientObserver
    {
        public Task VerifyQueueExitsAsync(CreateQueueOptions queueOptions)
        {
            return ForEachAsync(x => x.VerifyQueueExitsAsync(queueOptions));
        }

        public Task PreConsumerAsync(SubscriberContext context)
        {
            return ForEachAsync(x => x.PreConsumerAsync(context));
        }
    }
}