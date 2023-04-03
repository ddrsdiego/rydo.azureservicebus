namespace Rydo.AzureServiceBus.Client.Abstractions.Observers
{
    using System.Threading.Tasks;
    using Consumers.Subscribers;

    public interface IConsumerObserver
    {
        Task PreConsumerAsync(MessageContext context);

        Task PostConsumerAsync(MessageContext context);
    }
}