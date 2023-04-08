namespace Rydo.AzureServiceBus.Consumer.ConsumerHandlers
{
    using Client.Handlers;
    using Client.Topics;

    public class AccountUpdated
    {
        public AccountUpdated(string accountNumber)
        {
            CreatedAt = DateTime.Now;
            AccountNumber = accountNumber;
        }

        public DateTime CreatedAt { get; }

        public string AccountNumber { get; }
    }

    [TopicConsumer(typeof(AccountUpdated), TopicNameConstants.AccountUpdated)]
    public class AccountUpdatedConsumerHandler : IConsumerHandler<AccountUpdated>
    {
        public Task Consume(IConsumerContext<AccountUpdated> context)
        {
            throw new NotImplementedException();
        }
    }
}