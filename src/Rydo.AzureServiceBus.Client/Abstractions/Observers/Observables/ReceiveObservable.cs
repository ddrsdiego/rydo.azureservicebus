namespace Rydo.AzureServiceBus.Client.Abstractions.Observers.Observables
{
    using System;
    using System.Threading.Tasks;
    using Consumers.Subscribers;
    using Utils;

    internal class ReceiveObservable : Connectable<IReceiveObserver>, IReceiveObserver
    {
        public Task PreStartReceive(SubscriberContext context) => Count > 0
            ? ForEachAsync(x => x.PreStartReceive(context))
            : Task.CompletedTask;

        public Task PostStartReceive(SubscriberContext context) => Count > 0
            ? ForEachAsync(x => x.PostStartReceive(context))
            : Task.CompletedTask;

        public Task FaultStartReceive(SubscriberContext context, Exception exception) => Count > 0
            ? ForEachAsync(x => x.FaultStartReceive(context, exception))
            : Task.CompletedTask;

        public Task PreReceiveAsync(MessageContext context) => Count > 0
            ? ForEachAsync(x => x.PreReceiveAsync(context))
            : Task.CompletedTask;
    }
}