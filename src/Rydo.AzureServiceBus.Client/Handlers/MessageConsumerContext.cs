namespace Rydo.AzureServiceBus.Client.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Azure.Messaging.ServiceBus;
    using Consumers;
    using Subscribers;

    public sealed class MessageConsumerContext
    {
        private readonly object _syncLock;
        private readonly LinkedList<MessageContext> _messageContexts;

        public MessageConsumerContext(ConsumerContext subscriverContext, ServiceBusReceiver receiver,
            CancellationToken cancellationToken)
        {
            Count = 0;
            _syncLock = new object();
            CancellationToken = cancellationToken;
            SubscriverContext = subscriverContext;
            Receiver = receiver;
            _messageContexts = new LinkedList<MessageContext>();
        }

        internal readonly ServiceBusReceiver Receiver;
        internal readonly ConsumerContext SubscriverContext;
        internal readonly CancellationToken CancellationToken;

        internal Type HandlerType => SubscriverContext.HandlerType;
        internal Type ContractType => SubscriverContext.ContractType;

        internal void Add(MessageContext messageContext)
        {
            lock (_syncLock)
            {
                messageContext.SetMessageConsumerContext(this);
                _messageContexts.AddLast(messageContext);
                Count++;
            }
        }

        public int Count { get; private set; }

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