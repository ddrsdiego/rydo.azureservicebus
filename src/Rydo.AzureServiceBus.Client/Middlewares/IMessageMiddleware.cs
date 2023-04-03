namespace Rydo.AzureServiceBus.Client.Middlewares
{
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Handlers;

    public interface IMessageMiddleware :
        IConsumerObserverConnector,
        IConsumerMiddlewareObserverConnector
    {
        /// <summary>
        /// The method that is called when the middleware is invoked
        /// </summary>
        /// <param name="context">The message context</param>
        /// <param name="next">A delegate to the next middleware</param>
        /// <returns></returns>
        Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next);
    }
}