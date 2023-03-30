namespace Rydo.AzureServiceBus.Client.Configurations
{
    using System.Collections.Generic;

    public sealed class DeadLetterRetryEntry
    {
        public int Attempts { get; set; }
        public string Interval { get; set; }
    }
    
    public class DeadLetterPolicyEntry
    {
        public DeadLetterPolicyEntry() => Entries = new Dictionary<string, DeadLetterPolicyItem>();

        public IDictionary<string, DeadLetterPolicyItem> Entries { get; set; }
    }
    
    public class DeadLetterPolicyItem
    {
        public DeadLetterRetryEntry Retry { get; set; }

        public static DeadLetterRetryEntry EmptyRetry => new DeadLetterRetryEntry
        {
            Attempts = 0,
            Interval = string.Empty
        };
    }
}