namespace Rydo.AzureServiceBus.Client.Logging
{
    internal static class ServiceBusLogFields
    {
        public const string LogType = "{Log}";
        public const string TopicName = "{TopicName}";
        public const string SubscriptionName = "{SubscriptionName}";
        public const string MessageContextLog = "{@MessageContextLog}";
        public const string SubscriberContextLog = "{@SubscriberContextLog}";
        public const string MessageConsumerContextLength = "{MessageConsumercontextLength}";
        public const string ElapsedMilliseconds = "{ElapsedMilliseconds}";
        public const string MiddlewareType = "{MiddlewareTpe}";
        //CreateQueueOptions
        public const string CreateQueueOptions = "{@CreateQueueOptions}";
        public const string MessageConsumerContextAuditLog = "{@MsgConsumerContextAuditMedata}";
        
    }
}