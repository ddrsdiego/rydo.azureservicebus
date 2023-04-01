namespace Rydo.AzureServiceBus.Client.Middlewares
{
    using System.Threading.Tasks;
    using Abstractions.Observers.Observables;
    using Handlers;
    using Microsoft.Extensions.Logging;

    public interface IMessageMiddleware
    {
        ConsumerObservable ConsumerObservable { get; }
        ConsumerMiddlewareObservable ConsumerMiddlewareObservable { get; }
        /// <summary>
        /// The method that is called when the middleware is invoked
        /// </summary>
        /// <param name="context">The message context</param>
        /// <param name="next">A delegate to the next middleware</param>
        /// <returns></returns>
        Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next);
    }

    public abstract class MessageMiddleware : IMessageMiddleware
    {
        protected MessageMiddleware(ILogger logger)
        {
            Logger = logger;
            ConsumerObservable = new ConsumerObservable();
            ConsumerMiddlewareObservable = new ConsumerMiddlewareObservable();
        }

        protected ILogger Logger { get; }

        public ConsumerObservable ConsumerObservable { get; }
        public ConsumerMiddlewareObservable ConsumerMiddlewareObservable { get; }

        public abstract Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next);
    }
}