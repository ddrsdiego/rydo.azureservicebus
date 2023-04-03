namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Configurations.Host;
    using Middlewares;
    using Middlewares.Observers;

    public interface IReceiver :
        IReceiveObserverConnector,
        IFinishConsumerMiddlewareObserverConnector
    {
        IServiceBusClientWrapper BusClient { get; }
    }

    public interface IReceiverListener :
        IReceiver
    {
        IReceiverListener ServiceProvider(IServiceProvider serviceProvider);

        IReceiverListener MiddleExecutor(IMiddlewareExecutor middlewareExecutor);

        Task<bool> IsRunning { get; set; }

        Task<bool> StartAsync(CancellationToken stoppingToken);
        
        Task<bool> StopAsync(CancellationToken stoppingToken);
    }
}