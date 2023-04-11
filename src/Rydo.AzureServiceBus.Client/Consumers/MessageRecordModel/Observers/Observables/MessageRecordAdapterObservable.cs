namespace Rydo.AzureServiceBus.Client.Consumers.MessageRecordModel.Observers.Observables
{
    using System;
    using System.Threading.Tasks;
    using Rydo.AzureServiceBus.Client.Consumers.MessageRecordModel.Observers;
    using Rydo.AzureServiceBus.Client.Consumers.Subscribers;
    using Rydo.AzureServiceBus.Client.Utils;

    internal sealed class MessageRecordAdapterObservable :
        Connectable<IMessageRecordAdapterObserver>,
        IMessageRecordAdapterObserver
    {
        public Task PreAdapter(IMessageContext messageContext, Type contractType)
        {
            return ForEachAsync(x => x.PreAdapter(messageContext, contractType));
        }

        public Task FaultAdapter(IMessageContext messageContext, Type contractType, Exception exception)
        {
            return ForEachAsync(x => x.FaultAdapter(messageContext, contractType, exception));
        }
    }
}