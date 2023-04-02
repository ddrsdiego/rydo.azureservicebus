namespace Rydo.AzureServiceBus.Client.Abstractions.Observers
{
    using System.Threading.Tasks;
    using Handlers;

    public interface IConsumerMiddlewareObserver
    {
        Task PreConsumerAsync(string middlewareType, string step, MessageConsumerContext context);
        
        Task PostConsumerAsync(string middlewareType, string step, MessageConsumerContext context);
    }
}