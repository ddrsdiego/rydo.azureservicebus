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

    [TopicConsumer(TopicNameConstants.AccountCreatedTopic)]
    public class AccountCreatedConsumerHandler : ConsumerHandler<AccountCreated>
    {
        private readonly ILogger<AccountCreatedConsumerHandler> _logger;

        public AccountCreatedConsumerHandler(ILogger<AccountCreatedConsumerHandler> logger)
        {
            _logger = logger;
        }

        public override async Task HandleAsync(IMessageConsumerContext context,
            CancellationToken cancellationToken)
        {
            var tasks = new Task[context.Length];
            var messageRecords = context.Messages.ToArray();

            for (var index = 0; index < messageRecords.Length; index++)
            {
                var value = messageRecords[index].Value<AccountCreated>();
                tasks[index] = Task.Delay(100, cancellationToken);
            }

            for (var index = 0; index < tasks.Length; index++)
            {
                if (tasks[index].IsCompletedSuccessfully)
                    continue;
                await tasks[index];
            }
        }
    }
}