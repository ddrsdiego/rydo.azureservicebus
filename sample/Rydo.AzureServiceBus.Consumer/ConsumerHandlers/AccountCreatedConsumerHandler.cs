namespace Rydo.AzureServiceBus.Consumer.ConsumerHandlers
{
    using Client.Handlers;
    using Client.Topics;

    public class AccountCreated
    {
        public string AccountNumber { get; }
        public DateTime CreatedAt { get; }
        
        public AccountCreated(string accountNumber)
        {
            CreatedAt = DateTime.Now;
            AccountNumber = accountNumber;
        }   
    }
    
    [TopicConsumer("azureservicebus-sample-account-created")]
    public class AccountCreatedConsumerHandler : ConsumerHandler<AccountCreated>
    {
        private readonly ILogger<AccountCreatedConsumerHandler> _logger;

        public AccountCreatedConsumerHandler(ILogger<AccountCreatedConsumerHandler> logger)
        {
            _logger = logger;
        }
        
        public override async Task HandleAsync(MessageConsumerContext context)
        {
            foreach (var messageContext in context.MessageContexts)
            {
                var value = messageContext.MessageRecord.Value<AccountCreated>();
                await Task.Delay(250);
            }
        }
    }
}