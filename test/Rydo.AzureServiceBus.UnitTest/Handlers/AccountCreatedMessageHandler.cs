namespace Rydo.AzureServiceBus.UnitTest.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Client.Handlers.v2;
    using Consumer.ConsumerHandlers;

    public class AccountCreatedMessageHandler : ConsumerHandler<AccountCreated>
    {
        public override Task Handle(IMessageConsumerContext<AccountCreated> context,
            CancellationToken cancellationToken)
        {
            foreach (var message in context.Values)
            {
                var accountNumber = message.Value.AccountNumber;
            }

            return Task.CompletedTask;
        }
    }
}