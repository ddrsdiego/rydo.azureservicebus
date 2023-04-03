namespace Rydo.AzureServiceBus.Client.Configurations.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Middlewares.Consumers;
    using Middlewares.Extensions;
    using Services;

    internal static class ServiceBusClientConfiguratorEx
    {
        public static void Configure(this IServiceBusClientConfigurator configurator)
        {
            configurator.Services.AddMiddlewares();
            configurator.Services.TryAddSingleton<IMessageRecordFactory, MessageRecordFactory>();
            configurator.Services.TryAddSingleton(configurator.Receiver.ReceiverContextContainer);
            configurator.Services.TryAddSingleton(provider => provider.TryResolveReceiverListenerContainer(configurator));
            configurator.Services.TryAddSingleton(provider => provider.TryResolveSubscriberContextContainer(configurator));

            configurator.Services.AddHostedService<AzureServiceBusIntegrationHostedService>();
        }
    }
}