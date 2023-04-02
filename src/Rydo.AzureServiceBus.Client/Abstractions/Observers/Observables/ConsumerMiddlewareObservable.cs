namespace Rydo.AzureServiceBus.Client.Abstractions.Observers.Observables
{
    using System.Threading.Tasks;
    using Handlers;
    using Utils;

    public sealed class ConsumerMiddlewareObservable : Connectable<IConsumerMiddlewareObserver>,
        IConsumerMiddlewareObserver
    {
        public Task PreConsumerAsync(string middlewareType, string step, MessageConsumerContext context) =>
            Count <= 0
                ? Task.CompletedTask
                : ForEachAsync(x => x.PreConsumerAsync(middlewareType, step, context));

        public Task PostConsumerAsync(string middlewareType, string step, MessageConsumerContext context) =>
            Count <= 0
                ? Task.CompletedTask
                : ForEachAsync(x => x.PostConsumerAsync(middlewareType, step, context));
    }
}