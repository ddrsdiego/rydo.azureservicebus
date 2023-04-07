namespace Rydo.AzureServiceBus.Client.Handlers
{
    using Consumers.Subscribers;

    public interface IConsumerContext
    {
    }

    public interface IConsumerContext<TMessage> :
        IConsumerContext
    {
        MessageRecord<TMessage>[] Messages { get; }
    }
}