namespace Rydo.AzureServiceBus.Client.Handlers
{
    public interface IMessageConsumerContext :
        IConsumerContext
    {
        /// <summary>
        /// True if there is at least one message to be processed, false otherwise.
        /// </summary>
        bool AnyMessage { get; }

        /// <summary>
        /// Number of messages within the context to be processed.
        /// </summary>
        int Length { get; }
    }
}