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

    [TopicConsumer(TopicNameConstants.AccountUpdated)]
    public class AccountUpdatedConsumerHandler : ConsumerHandler<AccountUpdated>
    {
        public override Task HandleAsync(IMessageConsumerContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}