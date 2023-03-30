namespace Rydo.AzureServiceBus.Client.Subscribers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Middlewares;

    public interface ISubscriber
    {
        ISubscriber ServiceProvider(IServiceProvider serviceProvider);
        
        ISubscriber MiddleExecutor(IMiddlewareExecutor middlewareExecutor);
        
        ISubscriber ServiceBusClient(ServiceBusClient serviceBusClient);
        
        Task<bool> IsRunning { get; set; }

        Task<bool> StartAsync(CancellationToken stoppingToken);
    }
}