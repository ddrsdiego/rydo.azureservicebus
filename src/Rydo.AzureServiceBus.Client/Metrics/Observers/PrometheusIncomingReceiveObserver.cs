namespace Rydo.AzureServiceBus.Client.Metrics.Observers
{
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Consumers.Subscribers;

    internal sealed class PrometheusIncomingReceiveObserver : IReceiveObserver
    {
        public Task PreStartReceive(SubscriberContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task PostStartReceive(SubscriberContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}