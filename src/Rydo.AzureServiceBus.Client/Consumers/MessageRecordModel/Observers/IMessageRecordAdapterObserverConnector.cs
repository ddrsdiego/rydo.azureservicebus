namespace Rydo.AzureServiceBus.Client.Consumers.MessageRecordModel.Observers
{
    using Utils;

    public interface IMessageRecordAdapterObserverConnector
    {
        IConnectHandle ConnectMessageRecordAdapterObserver(IMessageRecordAdapterObserver observer);
    }
}