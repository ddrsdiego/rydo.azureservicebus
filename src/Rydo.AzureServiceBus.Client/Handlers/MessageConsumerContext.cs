namespace Rydo.AzureServiceBus.Client.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Configurations.Host;
    using Consumers.MessageRecordModel;
    using Consumers.Subscribers;
    using Utils;

    public sealed class MessageConsumerContext :
        IMessageConsumerContext
    {
        private int _length;
        private int _faultsLength;
        private readonly object _syncLock;

        private readonly Stopwatch _stopwatchMiddleware;
        private readonly Stopwatch _stopwatchMsgContext;
        private LinkedList<MessageRecord> _messagesRecordToRetry;
        private readonly LinkedList<IMessageContext> _messageContexts;

        internal MessageConsumerContext(SubscriberContext subscriberContext, IServiceBusClientReceiver receiver,
            CancellationToken cancellationToken)
        {
            _syncLock = new object();

            _stopwatchMsgContext = Stopwatch.StartNew();

            _stopwatchMiddleware = new Stopwatch();
            _messageContexts = new LinkedList<IMessageContext>();

            Length = 0;
            FaultsLength = 0;
            ContextId = GeneratorOperationId.Generate();

            CancellationToken = cancellationToken;
            SubscriberContext = subscriberContext;
            Receiver = receiver;

            Topic = subscriberContext.Specification.TopicName;
            Subscription = subscriberContext.Specification.SubscriptionName;
            Queue = subscriberContext.Specification.QueueName;
        }

        public string ContextId { get; }
        public string Queue { get; }
        public string Subscription { get; }
        public string Topic { get; }
        public IServiceBusClientReceiver Receiver { get; }
        public CancellationToken CancellationToken { get; }

        internal readonly SubscriberContext SubscriberContext;
        internal void StopMsgContextWatch() => _stopwatchMsgContext.Stop();
        internal void StopMiddlewareWatch() => _stopwatchMiddleware.Stop();
        internal void StarMiddlewareWatch() => _stopwatchMiddleware.Restart();
        internal long ElapsedTimeMiddleware => _stopwatchMiddleware.ElapsedMilliseconds;
        internal long ElapsedTimeMessageContext => _stopwatchMsgContext.ElapsedMilliseconds;
        internal Type HandlerType => SubscriberContext.HandlerType;
        internal Type ContractType => SubscriberContext.ContractType;
        internal bool AnyFault => FaultsLength > 0;
        internal int FaultsLength { get; private set; }

        internal void Add(MessageContext messageContext)
        {
            var id = Interlocked.Increment(ref _length);
            
            lock (_syncLock)
            {
                messageContext.SetMessageConsumerContext(this);
                _messageContexts.AddLast(messageContext);
                
                Length = id;
            }
        }

        internal void MarkToRetry(MessageRecord messageRecord, string reason) =>
            MarkToRetry(messageRecord, reason, null);

        internal void MarkToRetry(MessageRecord messageRecord, string reason, Exception exception)
        {
            if (messageRecord == null)
                return;

            _messagesRecordToRetry ??= new LinkedList<MessageRecord>();
            var id = Interlocked.Increment(ref _faultsLength);
            lock (_syncLock)
            {
                _messagesRecordToRetry.AddLast(messageRecord);
                FaultsLength = id;
            }
        }

        internal IMessageContext[] MessagesContext => _messageContexts.ToArray();

        internal IEnumerable<MessageRecord> Faults
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                foreach (var consumerRecord in _messagesRecordToRetry)
                    yield return consumerRecord;
            }
        }

        /// <summary>
        /// True if there is at least one message to be processed, false otherwise.
        /// </summary>
        public bool AnyMessage => Length > 0;

        /// <summary>
        /// Number of messages within the context to be processed.
        /// </summary>
        public int Length { get; private set; }
    }
}