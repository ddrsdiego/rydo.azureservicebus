namespace Rydo.AzureServiceBus.Client.Abstractions.Observers
{
    using System.Threading.Tasks;
    using Handlers;
    using Utils;

    public interface IConsumerMiddlewareObserver
    {
        Task PreConsumer(string middlewareType, string step, MessageConsumerContext context);
        Task PostConsumer(string middlewareType, string step, MessageConsumerContext context);
    }

    public interface IConsumerMiddlewareObserverConnector
    {
        IConnectHandle ConnectConsumerMiddlewareObserver(IConsumerMiddlewareObserver observer);
    }
}