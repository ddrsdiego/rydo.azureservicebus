namespace Rydo.AzureServiceBus.Client.Configurations.Subscribers
{
    using System;
    using System.Collections.Generic;
    using Rydo.AzureServiceBus.Client.Consumers.Subscribers;

    public interface IAzureServiceBusSubscribersConfigurator
    {
        void Configure(Type type, Action<ISubscriberContextContainer> container);
        
        void Configure(IEnumerable<Type> types, Action<ISubscriberContextContainer> container);
    }
}