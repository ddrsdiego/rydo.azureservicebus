namespace Rydo.AzureServiceBus.Client.Configurations.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Middlewares.Consumers;
    using Middlewares.Extensions;
    using Serialization;
    using Services;

    internal static class ServiceBusClientConfiguratorEx
    {
        public static void Configure(this IServiceBusClientConfigurator configurator)
        {
            configurator.Services.AddMiddlewares();
            configurator.Services.TryAddSingleton<IMessageRecordFactory, MessageRecordFactory>();
            configurator.Services.TryAddSingleton(configurator.Receiver.ReceiverContextContainer);
            configurator.Services.TryAddSingleton(configurator.TryResolveReceiverListenerContainer);
            configurator.Services.TryAddSingleton(configurator.TryResolveSubscriberContextContainer);

            configurator.Services.TryAddSingleton<ISerializer>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<SystemTextJsonSerializer>>();
                return new SystemTextJsonSerializer(logger);
            });

            configurator.Services.AddHostedService<AzureServiceBusIntegrationHostedService>();
        }
    }
}