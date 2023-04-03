namespace Rydo.AzureServiceBus.Client.Configurations.Extensions
{
    using System;
    using Abstractions.Observers;
    using Abstractions.Observers.Observables;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;
    using Consumers.Subscribers;
    using Host;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Receivers.Extensions;

    internal static class SubscriberContextContainerEx
    {
        public static ISubscriberContextContainer TryResolveSubscriberContextContainer(this IServiceProvider sp,
            IServiceBusClientConfigurator configurator)
        {
            foreach (var (topicName, consumerContext) in configurator.Receiver.ReceiverContextContainer.Subscriber
                         .Contexts)
            {
                var receiverListener = CreateReceiverListener(sp, consumerContext);
                configurator.Receiver.ListenerContainer.TryAddListener(topicName, receiverListener);
            }

            return configurator.Receiver.ReceiverContextContainer.Subscriber;
        }

        private static ReceiverListener CreateReceiverListener(IServiceProvider sp, SubscriberContext subscriberContext)
        {
            var logger = sp.GetRequiredService<ILoggerFactory>();
            var receiverListenerLogger = sp.GetRequiredService<ILogger<ReceiverListener>>();
            var serviceBusClient = sp.GetRequiredService<ServiceBusClient>();
            var serviceBusAdministrationClient = sp.GetRequiredService<ServiceBusAdministrationClient>();

            var hostSettings =
                new ServiceBusHostSettings("", serviceBusClient, serviceBusAdministrationClient);

            var client = new ServiceBusClientWrapper(hostSettings);
            client.Admin.ConnectAdminClientObservers(new LogAdminClientObserver(logger));
            
            var receiverListener = new ReceiverListener(receiverListenerLogger, client, subscriberContext);
            receiverListener.ConnectObservers(sp);
            
            return receiverListener;
        }
    }
}