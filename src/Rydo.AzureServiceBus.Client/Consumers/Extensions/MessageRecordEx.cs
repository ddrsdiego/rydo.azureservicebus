namespace Rydo.AzureServiceBus.Client.Consumers.Extensions
{
    using System;
    using Subscribers;

    public static class MessageRecordEx
    {
        /// <summary>
        /// Marks the message to be sent back to the consumer as long as the delivery count does not exceed the defined value.
        /// </summary>
        /// <param name="messageRecord"></param>
        /// <param name="reason">Reason why the message should be sent again. It will be logged for auditing.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void MarkToRetry(this MessageRecord messageRecord, string reason)
        {
            if (reason == null) throw new ArgumentNullException(nameof(reason));
            messageRecord.MarkToRetry(reason, null);
        }

        /// <summary>
        /// Marks the message to be sent back to the consumer as long as the delivery count does not exceed the defined value.
        /// </summary>
        /// <param name="messageRecord"></param>
        /// <param name="reason">Reason why the message should be sent again. It will be logged for auditing.</param>
        /// <param name="exception">Exception caught at the time of message processing. It will be logged for auditing.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void MarkToRetry(this MessageRecord messageRecord, string reason, Exception exception)
        {
            if (reason == null) throw new ArgumentNullException(nameof(reason));
            
            messageRecord.MessageConsumerCtx.MarkToRetry(messageRecord, reason, exception);
        }
    }
}