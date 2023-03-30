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
            var tasks = context.ConsumerRecords.Select(async message =>
            {
                var value = message.Value<AccountCreated>();
                
                _logger.LogInformation("{MessageId} - {MessagePayload}",
                    message.MessageId,
                    message.ValueAsJsonString());
                
                await Task.Delay(250);
            });

            await Task.WhenAll(tasks);
        }
    }
}