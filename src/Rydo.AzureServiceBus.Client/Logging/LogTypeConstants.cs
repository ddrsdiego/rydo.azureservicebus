namespace Rydo.AzureServiceBus.Client.Logging
{
    internal static class LogTypeConstants
    {
        public const string VerifyQueueExists = "verify-queue-exists";
        
        public const string StartReceiver = "start-receiver-listener";
        public const string ConnectedReceiver = "connected-receiver-listener";
        
        public const string StartReceiverFaulted = "start-receiver-listener-faulted";
        public const string ConnectedReceiverFaulted = "connected-receiver-listener-faulted";
        
        public const string IncomingMessageReceiver = "incoming-message";
        public const string CompleteMessageStep = "complete-consumer-message";
        public const string CustomHandlerConsumerStep = "custom-handler-consumer-messages";
        public const string DeadLetterConsumerStep = "dead-letter-consumer-messages";
        public const string DeserializerConsumerMessagesStep = "deserializer-consumer-messages";
    }
}