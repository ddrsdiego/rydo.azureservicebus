namespace Rydo.AzureServiceBus.Client.Abstractions.Observers
{
    using Utils;

    public interface IConsumerMiddlewareObserverConnector
    {
        IConnectHandle ConnectConsumerMiddlewareObserver(IConsumerMiddlewareObserver observer);
    }
}