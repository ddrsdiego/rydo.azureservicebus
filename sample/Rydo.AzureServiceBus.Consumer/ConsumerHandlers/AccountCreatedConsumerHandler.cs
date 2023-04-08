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

    [TopicConsumer(typeof(AccountCreated), TopicNameConstants.AccountCreated)]
    public class AccountCreatedConsumerHandler : IConsumerHandler<AccountCreated>
    {
        private readonly ILogger<AccountCreatedConsumerHandler> _logger;

        public AccountCreatedConsumerHandler(ILogger<AccountCreatedConsumerHandler> logger)
        {
            _logger = logger;
        }

        public async Task Consume(IConsumerContext<AccountCreated> context)
        {
            var tasks = new Task[context.Messages.Length];

            var messageRecords = context.Messages.ToArray();
            for (var index = 0; index < messageRecords.Length; index++)
            {
                var accountCreated = messageRecords[index].Value;
                tasks[index] = Task.Delay(100, context.CancellationToken);
            }

            for (var index = 0; index < tasks.Length; index++)
            {
                if (tasks[index].IsCompletedSuccessfully) continue;
                await tasks[index];
            }
        }
    }
}