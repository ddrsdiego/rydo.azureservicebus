namespace Rydo.AzureServiceBus.Client.Abstractions.Observers.Observables
{
    using System.Threading.Tasks;
    using Consumers.Subscribers;
    using Utils;

    internal class ReceiveObservable : Connectable<IReceiveObserver>, IReceiveObserver
    {
        public Task PreStartReceive(SubscriberContext context)
        {
            return ForEachAsync(x => x.PreStartReceive(context));
        }

        public Task PostStartReceive(SubscriberContext context)
        {
            return ForEachAsync(x => x.PostStartReceive(context));
        }
    }
}