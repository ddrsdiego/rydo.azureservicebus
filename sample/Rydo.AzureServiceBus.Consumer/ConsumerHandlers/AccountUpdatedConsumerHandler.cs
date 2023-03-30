namespace Rydo.AzureServiceBus.Consumer.ConsumerHandlers
{
    using Client.Handlers;
    using Client.Topics;

    public class AccountUpdated
    {
        public string AccountNumber { get; }
        public DateTime CreatedAt { get; }

        public AccountUpdated(string accountNumber)
        {
            CreatedAt = DateTime.Now;
            AccountNumber = accountNumber;
        }
    }

    [TopicConsumer(TopicNameConstants.AccountUpdatedTopic)]
    public class AccountUpdatedConsumerHandler : ConsumerHandler<AccountUpdated>
    {
        public override Task HandleAsync(MessageConsumerContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}