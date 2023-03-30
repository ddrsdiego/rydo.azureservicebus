﻿namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Middlewares;

    public interface IReceiverListener
    {
        IReceiverListener ServiceProvider(IServiceProvider serviceProvider);
        
        IReceiverListener MiddleExecutor(IMiddlewareExecutor middlewareExecutor);
        
        IReceiverListener ServiceBusClient(ServiceBusClient serviceBusClient);
        
        Task<bool> IsRunning { get; set; }

        Task<bool> StartAsync(CancellationToken stoppingToken);
    }
}