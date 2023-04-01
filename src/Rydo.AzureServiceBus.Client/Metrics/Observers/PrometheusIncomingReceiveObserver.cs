namespace Rydo.AzureServiceBus.Client.Metrics.Observers
{
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Consumers.Subscribers;

    internal sealed class PrometheusIncomingReceiveObserver : IReceiveObserver
    {
        public Task PreStartReceive(SubscriberContext context)
        {
            return Task.CompletedTask;
        }

        public Task PostStartReceive(SubscriberContext context)
        {
            return Task.CompletedTask;
        }

        public Task PreReceive(MessageContext context)
        {
            return Task.CompletedTask;
        }
    }
}