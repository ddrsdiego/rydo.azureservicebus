namespace Rydo.AzureServiceBus.Client.Abstractions.Observers
{
    using Utils;

    public interface IReceiveObserverConnector
    {
        IConnectHandle ConnectReceiveObserver(IReceiveObserver observer);
    }
}