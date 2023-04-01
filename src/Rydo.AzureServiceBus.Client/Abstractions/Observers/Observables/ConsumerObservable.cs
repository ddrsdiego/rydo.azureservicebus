namespace Rydo.AzureServiceBus.Client.Abstractions.Observers.Observables
{
    using System.Threading.Tasks;
    using Consumers.Subscribers;
    using Handlers;
    using Utils;

    public sealed class ConsumerObservable : Connectable<IConsumerObserver>, IConsumerObserver
    {
        public Task PreConsumer(MessageContext context)
        {
            return ForEachAsync(x => x.PreConsumer(context));
        }

        public Task PreConsumer(MessageConsumerContext context)
        {
            return ForEachAsync(x => x.PreConsumer(context));
        }

        public Task PostConsumer(MessageContext context)
        {
            return ForEachAsync(x => x.PostConsumer(context));
        }

        public Task PostConsumer(MessageConsumerContext context)
        {
            return ForEachAsync(x => x.PostConsumer(context));
        }
    }
}