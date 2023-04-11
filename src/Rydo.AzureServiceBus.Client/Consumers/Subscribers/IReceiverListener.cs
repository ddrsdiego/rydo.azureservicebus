namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Middlewares;
    using Middlewares.Observers;

    public interface IReceiver :
        IReceiveObserverConnector,
        IFinishConsumerMiddlewareObserverConnector
    {
    }

    public interface IReceiverListener :
        IReceiver
    {
        IReceiverListener ServiceProvider(IServiceProvider serviceProvider);

        IReceiverListener MiddleExecutor(IMiddlewareExecutor middlewareExecutor);

        Task<bool> IsRunning { get; set; }

        Task CreateEntitiesIfNotExistAsync(SubscriberContext subscriberContext, CancellationToken stoppingToken);

        Task<bool> StartAsync(CancellationTokenSource cancellationTokenSource);

        Task<bool> StopAsync(CancellationTokenSource cancellationTokenSource);
    }
}