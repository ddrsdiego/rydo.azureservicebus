namespace Rydo.AzureServiceBus.Client.Abstractions.Observers
{
    using Utils;

    public interface IConsumerObserverConnector
    {
        IConnectHandle ConnectConsumerObserver(IConsumerObserver observer);
    }
}