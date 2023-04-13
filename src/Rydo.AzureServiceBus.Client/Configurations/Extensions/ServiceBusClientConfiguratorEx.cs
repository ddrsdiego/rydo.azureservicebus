namespace Rydo.AzureServiceBus.Client.Configurations.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Middlewares.Extensions;
    using Services;

    internal static class ServiceBusClientConfiguratorEx
    {
        public static void Configure(this IServiceBusClientConfigurator configurator)
        {
            configurator.Collection.AddMiddlewares();
            configurator.Collection.TryAddSingleton(configurator.Receiver.ReceiverContextContainer);
            configurator.Collection.TryAddSingleton(configurator.TryResolveMessageRecordAdapter);
            configurator.Collection.TryAddSingleton(configurator.TryResolveReceiverListenerContainer);
            configurator.Collection.TryAddSingleton(configurator.TryResolveSubscriberContextContainer);
            configurator.Collection.TryAddSingleton(configurator.TryResolveSerializer);
            configurator.Collection.AddHostedService<AzureServiceBusIntegrationHostedService>();
        }
    }
}