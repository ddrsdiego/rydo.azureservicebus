namespace Rydo.AzureServiceBus.Client.Handlers
{
    using System.Threading;
    using Consumers.MessageRecordModel;

    internal sealed class ConsumerContext<TMessage> :
        IConsumerContext<TMessage>
        where TMessage : class
    {
        private readonly MessageConsumerContext _consumerContext;

        internal ConsumerContext(MessageConsumerContext consumerContext, MessageRecord<TMessage>[] messageRecords)
        {
            _consumerContext = consumerContext;
            Messages = messageRecords;
        }

        public MessageRecord<TMessage>[] Messages { get; }

        public string ContextId => _consumerContext.ContextId;
        public string Queue => _consumerContext.Queue;
        public string Subscription => _consumerContext.Subscription;
        public string Topic => _consumerContext.Topic;
        public CancellationToken CancellationToken => _consumerContext.CancellationToken;
    }
}