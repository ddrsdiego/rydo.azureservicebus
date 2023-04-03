namespace Rydo.AzureServiceBus.Client.Middlewares
{
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Abstractions.Observers.Observables;
    using Handlers;
    using Utils;

    internal abstract class MessageMiddleware : IMessageMiddleware
    {
        protected MessageMiddleware(string middlewareName)
        {
            MiddlewareName = middlewareName;
            ConsumerObservable = new ConsumerObservable();
            ConsumerMiddlewareObservable = new ConsumerMiddlewareObservable();
        }

        private string MiddlewareName { get; }
        
        private ConsumerMiddlewareObservable ConsumerMiddlewareObservable { get; }
        
        protected abstract string ConsumerMessagesStep { get; }

        protected ConsumerObservable ConsumerObservable { get; }

        protected abstract Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next);

        public IConnectHandle ConnectConsumerMiddlewareObserver(IConsumerMiddlewareObserver observer) =>
            ConsumerMiddlewareObservable.Connect(observer);

        public IConnectHandle ConnectConsumerObserver(IConsumerObserver observer) =>
            ConsumerObservable.Connect(observer);
        
        public async Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            if (!context.AnyMessage)
                await next(context);

            await ConsumerMiddlewareObservable.PreConsumerAsync(MiddlewareName, ConsumerMessagesStep, context);

            await ExecuteInvokeAsync(context, next);

            await ConsumerMiddlewareObservable.PostConsumerAsync(MiddlewareName, ConsumerMessagesStep, context);

            await next(context);
        }
    }
}