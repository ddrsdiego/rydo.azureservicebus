namespace Rydo.AzureServiceBus.Producer.ConsumerHandlers
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
    
    [TopicConsumer(typeof(AccountCreated), TopicNameConstants.AccountCreated)]
    public sealed class AccountCreatedConsumerHandler : IConsumerHandler<AccountCreated>
    {
        private readonly ILogger<AccountCreatedConsumerHandler> _logger;

        public AccountCreatedConsumerHandler(ILogger<AccountCreatedConsumerHandler> logger)
        {
            _logger = logger;
        }

        public Task Consume(IConsumerContext<AccountCreated> context)
        {
            return Task.CompletedTask;
        }
    }
}