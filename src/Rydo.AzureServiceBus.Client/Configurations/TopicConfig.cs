namespace Rydo.AzureServiceBus.Client.Configurations
{
    public class TopicConfig
    {
        public string Name { get; set; }

        public string Direction { get; set; }
        
        public string SubscriptionName { get; set; }
        
        public string DeadLetterPolicyName { get; set; }
    }
}