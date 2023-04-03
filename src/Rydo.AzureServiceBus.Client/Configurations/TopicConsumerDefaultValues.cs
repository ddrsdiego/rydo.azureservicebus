namespace Rydo.AzureServiceBus.Client.Configurations
{
    internal static class TopicConsumerDefaultValues
    {
        public const int MaxMessages = 1;
        public const int BufferSize = 1;
        public const int PrefetchCount  = 100;
        public const int MaxDeliveryCount = 10;
        public const int LockDurationInSeconds = 300;
        public const bool NeverAutoDelete = false;
        public const int AutoDeleteAfterIdleInHours = 240;
    }
}