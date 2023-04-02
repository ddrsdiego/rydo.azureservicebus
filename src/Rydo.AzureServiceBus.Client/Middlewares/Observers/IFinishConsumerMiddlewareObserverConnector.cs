namespace Rydo.AzureServiceBus.Client.Middlewares.Observers
{
    using Utils;

    public interface IFinishConsumerMiddlewareObserverConnector
    {
        IConnectHandle ConnectFinishConsumerMiddlewareObserver(IFinishConsumerMiddlewareObserver observer);
    }
}