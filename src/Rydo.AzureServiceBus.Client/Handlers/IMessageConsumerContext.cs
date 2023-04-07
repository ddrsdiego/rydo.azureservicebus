namespace Rydo.AzureServiceBus.Client.Handlers
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Consumers.Subscribers;

    public interface IMessageConsumerContext
    {
        string ContextId { get; }

        /// <summary>
        /// True if there is at least one message to be processed, false otherwise.
        /// </summary>
        bool AnyMessage { get; }

        /// <summary>
        /// Number of messages within the context to be processed.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// List of messages to be processed. The Length and AnyMessage properties indicate if there are messages in the list.
        /// </summary>
        IEnumerable<MessageRecord> Messages
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}