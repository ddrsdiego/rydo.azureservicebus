namespace Rydo.AzureServiceBus.Client.Middlewares.Observers
{
    using System.Threading.Tasks;
    using Handlers;

    public interface IFinishConsumerMiddlewareObserver
    {
        Task EndConsumerAsync(MessageConsumerContext context);
    }
}