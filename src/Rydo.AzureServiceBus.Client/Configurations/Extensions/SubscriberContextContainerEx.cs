namespace Rydo.AzureServiceBus.Client.Configurations.Extensions
{
    using System;
    using Abstractions.Observers;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;
    using Consumers.Subscribers;
    using Host;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Receivers.Extensions;

    internal static class SubscriberContextContainerEx
    {
        public static ISubscriberContextContainer TryResolveSubscriberContextContainer(
            this IServiceBusClientConfigurator configurator, IServiceProvider sp)
        {
            foreach (var (topicName, consumerContext) in configurator.Receiver.ReceiverContextContainer.Subscriber
                         .Contexts)
            {
                var receiverListener = CreateReceiverListener(sp, consumerContext);
                configurator.Receiver.ListenerContainer.TryAddListener(topicName, receiverListener);
            }

            return configurator.Receiver.ReceiverContextContainer.Subscriber;
        }

        private static IReceiverListener CreateReceiverListener(IServiceProvider serviceProvider,
            SubscriberContext subscriberContext)
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>();
            var serviceBusClient = serviceProvider.GetRequiredService<ServiceBusClient>();
            var serviceBusAdministrationClient = serviceProvider.GetRequiredService<ServiceBusAdministrationClient>();

            var hostSettings =
                new ServiceBusHostSettings(serviceBusClient, serviceBusAdministrationClient);

            var serviceBusClientWrapper = new ServiceBusClientWrapper(logger, hostSettings);
            serviceBusClientWrapper.Admin.ConnectAdminClientObservers(new LogAdminClientObserver(logger));

            var receiverListener = new ReceiverListener(serviceBusClientWrapper, subscriberContext);
            receiverListener.ConnectObservers(serviceProvider);

            return receiverListener;
        }
    }
}