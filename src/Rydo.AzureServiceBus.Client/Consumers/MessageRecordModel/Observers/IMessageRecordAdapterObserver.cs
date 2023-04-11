namespace Rydo.AzureServiceBus.Client.Consumers.MessageRecordModel.Observers
{
    using System;
    using System.Threading.Tasks;
    using Subscribers;

    public interface IMessageRecordAdapterObserver
    {
        Task PreAdapter(IMessageContext messageContext, Type contractType);

        Task FaultAdapter(IMessageContext messageContext, Type contractType, Exception exception);
    }
}