namespace Rydo.AzureServiceBus.Client.Configurations.Extensions
{
    using System;
    using Consumers.Subscribers;

    internal static class ReceiverListenerContainerEx
    {
        public static IReceiverListenerContainer TryResolveReceiverListenerContainer(this IServiceProvider sp,
            IServiceBusClientConfigurator configurator)
        {
            configurator.Receiver.ListenerContainer.SetServiceProvider(sp);
            return configurator.Receiver.ListenerContainer;
        }
    }
}