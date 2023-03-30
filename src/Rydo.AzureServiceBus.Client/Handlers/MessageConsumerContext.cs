namespace Rydo.AzureServiceBus.Client.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Azure.Messaging.ServiceBus;
    using Consumers;

    public sealed class MessageConsumerContext
    {
        private readonly object _syncLock;
        private readonly LinkedList<MessageContext> _messageContexts;

        public MessageConsumerContext(ConsumerContext consumerContext, ServiceBusReceiver receiver)
        {
            Count = 0;
            _syncLock = new object();
            ConsumerContext = consumerContext;
            Receiver = receiver;
            _messageContexts = new LinkedList<MessageContext>();
        }

        internal readonly ServiceBusReceiver Receiver;
        internal readonly ConsumerContext ConsumerContext;
        internal Type HandlerType => ConsumerContext.HandlerType;
        internal Type ContractType => ConsumerContext.ContractType;
        
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

        public IEnumerable<MessageRecord> ConsumerRecords
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                foreach (var messageContext in _messageContexts)
                {
                    // if (messageContext.MessageRecord.IsValid)
                    // {
                    //     
                    // }
                    
                    yield return messageContext.MessageRecord;
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