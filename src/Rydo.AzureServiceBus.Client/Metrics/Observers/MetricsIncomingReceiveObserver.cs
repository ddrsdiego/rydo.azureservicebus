namespace Rydo.AzureServiceBus.Client.Metrics.Observers
{
    using System;
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Consumers.Subscribers;

    internal sealed class MetricsIncomingReceiveObserver :
        IReceiveObserver
    {
        public Task PreStartReceive(SubscriberContext context)
        {
            return Task.CompletedTask;
        }

        public Task PostStartReceive(SubscriberContext context)
        {
            return Task.CompletedTask;
        }

        public Task FaultStartReceive(SubscriberContext context, Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task PreReceiveAsync(MessageContext context)
        {
            return Task.CompletedTask;
        }
    }
}