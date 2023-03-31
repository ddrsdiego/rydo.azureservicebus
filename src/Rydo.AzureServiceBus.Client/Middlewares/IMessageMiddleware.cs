namespace Rydo.AzureServiceBus.Client.Middlewares
{
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.Logging;

    public interface IMessageMiddleware
    {
        /// <summary>
        /// The method that is called when the middleware is invoked
        /// </summary>
        /// <param name="context">The message context</param>
        /// <param name="next">A delegate to the next middleware</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next);
    }

    public abstract class MessageMiddleware : IMessageMiddleware
    {
        protected MessageMiddleware(ILogger logger)
        {
            Logger = logger;
        }
        
        protected ILogger Logger { get; }
        
        public abstract Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next);
    }
}