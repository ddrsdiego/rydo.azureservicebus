namespace Rydo.AzureServiceBus.Client.Configurations
{
    internal static class TopicConsumerDefaultValues
    {
        public const int MaxMessages = 1;
        public const int BufferSize = 1;
        public const int MaxDeliveryCount = 10;
        public const int LockDurationInSeconds = 60;
    }
}