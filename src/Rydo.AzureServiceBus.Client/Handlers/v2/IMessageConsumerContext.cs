namespace Rydo.AzureServiceBus.Client.Handlers.v2
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Consumers.Subscribers;

    public interface IMessageConsumerContext<TMessage> :
        IMessageConsumerContext
    {
        /// <summary>
        /// List of messages to be processed. The Length and AnyMessage properties indicate if there are messages in the list.
        /// </summary>
        IEnumerable<MessageRecord<TMessage>> Values
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
    
    public sealed class MessageConsumerContext<TMessage> : IMessageConsumerContext<TMessage>
    {
        public string ContextId { get; }
        
        public bool AnyMessage { get; }
        
        public int Length { get; }
        
        public string QueueSubscription { get; }
        
        public IEnumerable<MessageRecord> Messages { get; }
        
        public IEnumerable<MessageRecord<TMessage>> Values { get; }
    }
}