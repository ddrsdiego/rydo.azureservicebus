namespace Rydo.AzureServiceBus.Client.Handlers
{
    using System.Threading;
    using Consumers.MessageRecordModel;

    public sealed class ConsumeContextScope<TMessage> :
        IConsumerContext<TMessage>
        where TMessage : class
    {
        private readonly IConsumerContext<TMessage> _consumerContext;

        public ConsumeContextScope(IConsumerContext<TMessage> consumerContext)
        {
            _consumerContext = consumerContext;
        }

        public string ContextId => _consumerContext.ContextId;
        public string Queue => _consumerContext.Queue;
        public string Subscription => _consumerContext.Subscription;
        public string Topic => _consumerContext.Topic;
        public CancellationToken CancellationToken => _consumerContext.CancellationToken;
        public MessageRecord<TMessage>[] Messages => _consumerContext.Messages;
    }
}