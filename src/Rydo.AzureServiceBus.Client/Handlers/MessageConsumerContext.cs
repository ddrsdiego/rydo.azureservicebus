namespace Rydo.AzureServiceBus.Client.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Azure.Messaging.ServiceBus;
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
        private readonly LinkedList<MessageContext> _messageContexts;

        internal MessageConsumerContext(SubscriberContext subscriberContext, ServiceBusReceiver receiver,
            CancellationToken cancellationToken)
        {
            _syncLock = new object();

            _stopwatchMsgContext = Stopwatch.StartNew();

            _stopwatchMiddleware = new Stopwatch();
            _messageContexts = new LinkedList<MessageContext>();

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

        public readonly string Topic;
        public readonly string Subscription;
        public readonly string Queue;

        internal readonly ServiceBusReceiver Receiver;
        internal readonly SubscriberContext SubscriberContext;
        internal readonly CancellationToken CancellationToken;

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

        internal IEnumerable<MessageContext> MessagesContext
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                foreach (var messageContext in _messageContexts)
                    yield return messageContext;
            }
        }

        internal IEnumerable<MessageRecord> Faults
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                foreach (var consumerRecord in _messagesRecordToRetry)
                    yield return consumerRecord;
            }
        }

        public string ContextId { get; }

        /// <summary>
        /// True if there is at least one message to be processed, false otherwise.
        /// </summary>
        public bool AnyMessage => Length > 0;

        /// <summary>
        /// Number of messages within the context to be processed.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// List of messages to be processed. The Length and AnyMessage properties indicate if there are messages in the list.
        /// </summary>
        public IEnumerable<MessageRecord> Messages
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                foreach (var messageContext in _messageContexts)
                {
                    if (messageContext.Record.IsInvalid) continue;
                    yield return messageContext.Record;
                }
            }
        }
    }
}