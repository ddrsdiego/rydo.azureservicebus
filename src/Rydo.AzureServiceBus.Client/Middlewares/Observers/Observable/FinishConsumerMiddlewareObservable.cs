namespace Rydo.AzureServiceBus.Client.Middlewares.Observers.Observable
{
    using System.Threading.Tasks;
    using Handlers;
    using Utils;

    internal sealed class FinishConsumerMiddlewareObservable : Connectable<IFinishConsumerMiddlewareObserver>,
        IFinishConsumerMiddlewareObserver
    {
        public Task EndConsumerAsync(MessageConsumerContext context) =>
            Count <= 0 ?
                Task.CompletedTask
                : ForEachAsync(x => x.EndConsumerAsync(context));
    }
}