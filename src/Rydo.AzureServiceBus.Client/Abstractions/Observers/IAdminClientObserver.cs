namespace Rydo.AzureServiceBus.Client.Abstractions.Observers
{
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus.Administration;
    using Consumers.Subscribers;
    using Utils;

    public interface IAdminClientObserver
    {
        Task VerifyQueueExitsAsync(CreateQueueOptions queueOptions);
        
        Task PreConsumerAsync(SubscriberContext context);
    }

    public interface IAdminObserverConnector
    {
        IConnectHandle ConnectAdminClientObservers(IAdminClientObserver clientObserver);
    }
}