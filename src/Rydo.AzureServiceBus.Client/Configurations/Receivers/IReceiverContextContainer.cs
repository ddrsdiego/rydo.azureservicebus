namespace Rydo.AzureServiceBus.Client.Configurations.Receivers
{
    using Consumers.Subscribers;
    using Microsoft.Extensions.DependencyInjection;

    public interface IReceiverContextContainer
    {
        ISubscriberContextContainer Subscriber { get; }
    }

    internal sealed class ReceiverContextContainer : IReceiverContextContainer
    {
        public ReceiverContextContainer(IServiceCollection services) => Subscriber = new SubscriberContextContainer(services);
        public ISubscriberContextContainer Subscriber { get; }
    }
}