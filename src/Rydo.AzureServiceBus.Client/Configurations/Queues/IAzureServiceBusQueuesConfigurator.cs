namespace Rydo.AzureServiceBus.Client.Configurations.Queues
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Rydo.AzureServiceBus.Client.Consumers.Queues;

    public interface IAzureServiceBusQueuesConfigurator
    {
        void Configure(Type type, Action<IQueueContextContainer> container);
    }
    
    public class AzureServiceBusQueuesConfigurator : IAzureServiceBusQueuesConfigurator
    {
        private readonly IServiceCollection _services;

        public AzureServiceBusQueuesConfigurator(IServiceCollection services)
        {
            _services = services;
        }
        
        public void Configure(Type type, Action<IQueueContextContainer> container)
        {
            throw new NotImplementedException();
        }
    }
}