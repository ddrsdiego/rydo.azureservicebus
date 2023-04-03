namespace Rydo.AzureServiceBus.Client.Abstractions.Observers.Observables
{
    using System.Threading.Tasks;
    using Consumers.Subscribers;
    using Utils;

    internal sealed class ConsumerObservable : Connectable<IConsumerObserver>, IConsumerObserver
    {
        public Task PreConsumerAsync(MessageContext context) =>
            Count > 0
                ? ForEachAsync(x => x.PreConsumerAsync(context))
                : Task.CompletedTask;

        public Task PostConsumerAsync(MessageContext context) =>
            Count > 0
                ? ForEachAsync(x => x.PostConsumerAsync(context))
                : Task.CompletedTask;
    }
}