namespace Rydo.AzureServiceBus.Client.Middlewares
{
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Abstractions.Observers.Observables;
    using Handlers;
    using Utils;

    internal abstract class MessageMiddleware : IMessageMiddleware
    {
        private readonly ConsumerMiddlewareObservable _consumerMiddlewareObservable;

        protected MessageMiddleware(string middlewareName)
        {
            MiddlewareName = middlewareName;
            ConsumerObservable = new ConsumerObservable();
            _consumerMiddlewareObservable = new ConsumerMiddlewareObservable();
        }

        private string MiddlewareName { get; }

        protected abstract string ConsumerMessagesStep { get; }

        public ConsumerObservable ConsumerObservable { get; }

        public IConnectHandle ConnectConsumerMiddlewareObserver(IConsumerMiddlewareObserver observer) =>
            _consumerMiddlewareObservable.Connect(observer);

        public async Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            if (!context.AnyMessage) await next(context);
            
            await _consumerMiddlewareObservable.PreConsumerAsync(MiddlewareName, ConsumerMessagesStep, context);
            
            await ExecuteInvokeAsync(context, next);
            
            await _consumerMiddlewareObservable.PostConsumerAsync(MiddlewareName, ConsumerMessagesStep, context);
            
            await next(context);
        }

        protected abstract Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next);
    }
}