namespace Rydo.AzureServiceBus.Client.Configurations.Receivers
{
    using System;
    using System.Collections.Generic;

    public interface IAzureServiceBusReceiverConfigurator
    {
        void Configure(Type type, Action<IReceiverContextContainer> container);
        
        void Configure(IEnumerable<Type> types, Action<IReceiverContextContainer> container);

        void Configure(Type type, string queueName, Action<IReceiverContextContainer> container);
        
        void Configure(IEnumerable<Type> types, string queueName, Action<IReceiverContextContainer> container);
    }
}