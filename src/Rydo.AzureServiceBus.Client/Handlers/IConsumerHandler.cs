namespace Rydo.AzureServiceBus.Client.Handlers
{
    using System.Threading.Tasks;

    public interface IConsumerHandler
    {
    }

    public interface IConsumerHandler<TMessage> :
        IConsumerHandler
    {
        Task Consume(IConsumerContext<TMessage> context);
    }
}