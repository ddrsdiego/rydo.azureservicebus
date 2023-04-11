namespace Rydo.AzureServiceBus.Client.Consumers.Extensions
{
    using System;
    using MessageRecordModel;

    public static class MessageRecordEx
    {
        /// <summary>
        /// Marks the message to be sent back to the consumer as long as the delivery count does not exceed the defined value.
        /// </summary>
        /// <param name="messageRecord"></param>
        /// <param name="reason">Reason why the message should be sent again. It will be logged for auditing.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void MarkToRetry<TMessage>(this MessageRecord<TMessage> messageRecord, string reason)
            where TMessage : class
        {
            if (reason == null) throw new ArgumentNullException(nameof(reason));
            messageRecord.MarkToRetry(reason, null);
        }

        public static void MarkToRetry<TMessage>(this MessageRecord<TMessage> messageRecord, string reason,
            Exception exception) where TMessage : class
        {
            messageRecord.Message.MessageConsumerCtx.MarkToRetry(messageRecord.Message, reason, exception);
        }
    }
}