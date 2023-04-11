namespace Rydo.AzureServiceBus.Client.Handlers
{
    using System.Threading;
    using Consumers.MessageRecordModel;

    public interface IConsumerContext
    {
        public string ContextId { get; }
        public string Queue { get; }
        public string Subscription { get; }
        public string Topic { get; }
        CancellationToken CancellationToken { get; }
    }

    public interface IConsumerContext<TMessage> :
        IConsumerContext
        where TMessage : class
    {
        MessageRecord<TMessage>[] Messages { get; }
    }
}