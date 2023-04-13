namespace Rydo.AzureServiceBus.Client.Configurations.Extensions
{
    using System;
    using Consumers.MessageRecordModel;
    using Consumers.MessageRecordModel.Observers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serialization;

    internal static class MessageRecordAdapterResolverEx
    {
        public static IMessageRecordAdapter TryResolveMessageRecordAdapter(
            this IServiceBusClientConfigurator configurator, IServiceProvider sp)
        {
            var logger = sp.GetRequiredService<ILoggerFactory>();
            var serializer = sp.GetRequiredService<ISerializer>();

            var messageRecordAdapter = new MessageRecordAdapter(serializer);
            messageRecordAdapter.ConnectMessageRecordAdapterObserver(new MetricsMessageRecordAdapterObserver());
            messageRecordAdapter.ConnectMessageRecordAdapterObserver(new LogMessageRecordAdapterObserver(logger));

            return messageRecordAdapter;
        }
    }
}