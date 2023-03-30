namespace Rydo.AzureServiceBus.UnitTest.Fakes
{
    using Client.Topics;
    using Newtonsoft.Json;

    [TopicProducer(TopicName)]
    public class AccountCreatedMessage
    {
        public const string TopicName = "rydo.accounts.account-created";

        public AccountCreatedMessage(string accountNumber) => AccountNumber = accountNumber;

        public string AccountNumber { get; }
    }

    
    [TopicProducer(TopicName)]
    public class AccountUpdatedMessage
    {
                
        public const string TopicName = "rydo.accounts.account-updated";

        public AccountUpdatedMessage(string accountNumber) => AccountNumber = accountNumber;

        public string AccountNumber { get; }
    }
    
    [TopicProducer("")]
    public class AccountCreatedInvalidMessage
    {
    }
    
    [JsonObject]
    public class AccountCreatedWithoutTopicMessage
    {
    }
}