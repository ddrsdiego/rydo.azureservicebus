namespace Rydo.AzureServiceBus.Client.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Azure.Messaging.ServiceBus;
    using Consumers.Subscribers;

    public sealed class MessageConsumerContext
    {
        private int _count;
        private readonly object _syncLock;
        private readonly Stopwatch _stopwatch;
        private readonly LinkedList<MessageContext> _messageContexts;

        public MessageConsumerContext(SubscriberContext subscriberContext, ServiceBusReceiver receiver,
            CancellationToken cancellationToken)
        {
            Count = 0;
            _syncLock = new object();
            _stopwatch = new Stopwatch();
            CancellationToken = cancellationToken;
            SubscriberContext = subscriberContext;
            Receiver = receiver;
            _messageContexts = new LinkedList<MessageContext>();
        }
        
        internal readonly ServiceBusReceiver Receiver;
        internal readonly SubscriberContext SubscriberContext;
        internal readonly CancellationToken CancellationToken;

        internal void StarWatch() => _stopwatch.Restart();
        internal void StopWatch() => _stopwatch.Stop();
        internal long ElapsedTimeConsumer => _stopwatch.ElapsedMilliseconds;

        internal Type HandlerType => SubscriberContext.HandlerType;
        internal Type ContractType => SubscriberContext.ContractType;

        internal void Add(MessageContext messageContext)
        {
            var id = Interlocked.Increment(ref _count);
            lock (_syncLock)
            {
                messageContext.SetMessageConsumerContext(this);
                _messageContexts.AddLast(messageContext);
                Count = id;
            }
        }

        public int Count { get; private set; }

        public bool AnyMessage => Count > 0;

        public IEnumerable<MessageRecord> Messages
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                foreach (var messageContext in _messageContexts)
                {
                    if (messageContext.Message.IsInvalid) continue;
                    yield return messageContext.Message;
                }
            }
        }

        internal IEnumerable<MessageContext> MessageContexts
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                foreach (var messageContext in _messageContexts)
                {
                    yield return messageContext;
                }
            }
        }
    }
}