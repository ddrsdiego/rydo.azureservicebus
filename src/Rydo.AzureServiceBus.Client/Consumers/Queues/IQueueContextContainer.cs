namespace Rydo.AzureServiceBus.Client.Consumers.Queues
{
    public interface IQueueContextContainer
    {
        void QueueName(string empty);
        void MaxDeliveryCount(int i);
        void LockDurationInMinutes(int i);
        void AutoDeleteAfterIdleInHours(int i);
    }
}