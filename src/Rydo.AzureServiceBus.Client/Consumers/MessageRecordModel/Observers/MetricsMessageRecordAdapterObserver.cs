namespace Rydo.AzureServiceBus.Client.Consumers.MessageRecordModel.Observers
{
    using System;
    using System.Threading.Tasks;
    using Subscribers;

    internal sealed class MetricsMessageRecordAdapterObserver : IMessageRecordAdapterObserver
    {
        public Task PreAdapter(IMessageContext messageContext, Type contractType)
        {
            return Task.CompletedTask;
        }

        public Task FaultAdapter(IMessageContext messageContext, Type contractType, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}