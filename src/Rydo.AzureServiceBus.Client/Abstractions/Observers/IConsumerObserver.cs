namespace Rydo.AzureServiceBus.Client.Abstractions.Observers
{
    using System.Threading.Tasks;
    using Consumers.Subscribers;
    using Handlers;

    public interface IConsumerObserver
    {
        Task PreConsumer(MessageContext context);

        Task PreConsumer(MessageConsumerContext context);

        Task PostConsumer(MessageContext context);

        Task PostConsumer(MessageConsumerContext context);
    }
}