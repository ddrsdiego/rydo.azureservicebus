namespace Rydo.AzureServiceBus.Client.Consumers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Microsoft.Extensions.Logging;
    using Middlewares;

    public interface ISubscriber
    {
        ISubscriber WithServiceProvider(IServiceProvider serviceProvider);
        
        ISubscriber WithMiddleExecutor(IMiddlewareExecutor middlewareExecutor);
        
        ISubscriber WithServiceBusClient(ServiceBusClient serviceBusClient);

        ISubscriber WithLogging(ILogger<Subscriber> logger);
        
        Task<bool> IsRunning { get; set; }

        Task<bool> StartAsync(CancellationToken stoppingToken);
    }
}