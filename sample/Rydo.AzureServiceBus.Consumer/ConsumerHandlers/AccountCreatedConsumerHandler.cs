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

        public override async Task HandleAsync(MessageConsumerContext context,
            CancellationToken cancellationToken = default)
        {
            foreach (var message in context.Messages)
            {
                var value = message.Value<AccountCreated>();

                // _logger.LogInformation("{MessageId} - {MessagePayload}",
                //     message.MessageId,
                //     message.ValueAsJsonString());

                await Task.Delay(250, cancellationToken);
            }
        }
    }
}