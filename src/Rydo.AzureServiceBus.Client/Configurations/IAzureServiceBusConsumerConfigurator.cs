namespace Rydo.AzureServiceBus.Client.Configurations
{
    using System;
    using System.Collections.Generic;
    using Consumers;

    public interface IAzureServiceBusConsumerConfigurator
    {
        void Configure(Type type, Action<IConsumerContextContainer> container);
        
        void Configure(IEnumerable<Type> types, Action<IConsumerContextContainer> container);
    }
}