namespace Rydo.AzureServiceBus.Client.Configurations.Extensions
{
    using Consumers.MessageRecordModel;
    using Consumers.MessageRecordModel.Observers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Middlewares.Extensions;
    using Serialization;
    using Services;

    internal static class ServiceBusClientConfiguratorEx
    {
        public static void Configure(this IServiceBusClientConfigurator configurator)
        {
            configurator.Services.AddMiddlewares();
            configurator.Services.TryAddSingleton(configurator.Receiver.ReceiverContextContainer);
            configurator.Services.TryAddSingleton(configurator.TryResolveReceiverListenerContainer);
            configurator.Services.TryAddSingleton(configurator.TryResolveSubscriberContextContainer);
            configurator.Services.TryAddSingleton<IMessageRecordAdapter>(sp =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>();
                var serializer = sp.GetRequiredService<ISerializer>();

                var messageRecordAdapter = new MessageRecordAdapter(serializer);
                messageRecordAdapter.ConnectMessageRecordAdapterObserver(new LogMessageRecordAdapterObserver(logger));
                messageRecordAdapter.ConnectMessageRecordAdapterObserver(new MetricsMessageRecordAdapterObserver());

                return messageRecordAdapter;
            });

            configurator.Services.TryAddSingleton<ISerializer>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<SystemTextJsonSerializer>>();
                return new SystemTextJsonSerializer(logger);
            });

            configurator.Services.AddHostedService<AzureServiceBusIntegrationHostedService>();
        }
    }
}