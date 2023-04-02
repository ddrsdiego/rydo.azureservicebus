namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Azure.Messaging.ServiceBus;
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

        IReceiverListener ServiceBusClient(ServiceBusClient serviceBusClient);

        Task<bool> IsRunning { get; set; }

        Task<bool> StartAsync(CancellationToken stoppingToken);
        
        Task<bool> StopAsync(CancellationToken stoppingToken);
    }
}