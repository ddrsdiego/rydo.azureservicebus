namespace Rydo.AzureServiceBus.Client.Abstractions.Observers.Observables
{
    using System.Threading.Tasks;
    using Handlers;
    using Utils;

    public class ConsumerMiddlewareObservable : Connectable<IConsumerMiddlewareObserver>, IConsumerMiddlewareObserver
    {
        public Task PreConsumer(string middlewareType, string step, MessageConsumerContext context)
        {
            return ForEachAsync(x => x.PreConsumer(middlewareType, step, context));
        }

        public Task PostConsumer(string middlewareType, string step, MessageConsumerContext context)
        {
            return ForEachAsync(x => x.PostConsumer(middlewareType, step, context));
        }
    }
}