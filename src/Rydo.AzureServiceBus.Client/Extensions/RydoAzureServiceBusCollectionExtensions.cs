namespace Rydo.AzureServiceBus.Client.Extensions
{
    using System;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;
    using Configurations;
    using Configurations.Host;
    using Configurations.Receivers.Extensions;
    using Consumers.Subscribers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Middlewares.Consumers;
    using Middlewares.Extensions;
    using Services;

    public static class RydoAzureServiceBusCollectionExtensions
    {
        public static void AddAzureServiceBusClient(this IServiceCollection services,
            Action<IServiceBusClientConfigurator> clientConfigurator)
        {
            var configurator = new ServiceBusClientConfigurator(services);
            clientConfigurator(configurator);

            services.AddMiddlewares();
            services.TryAddSingleton<IMessageRecordFactory, MessageRecordFactory>();
            services.AddHostedService<AzureServiceBusIntegrationHostedService>();

            services.TryAddSingleton(configurator.Receiver.ReceiverContextContainer);
            services.TryAddSingleton(provider => TryResolveReceiverListenerContainer(provider, configurator));
            services.TryAddSingleton(provider => TryResolveSubscriberContextContainer(provider, configurator));
        }

        private static IReceiverListenerContainer TryResolveReceiverListenerContainer(IServiceProvider sp,
            IServiceBusClientConfigurator configurator)
        {
            configurator.Receiver.ReceiverListenerContainer.SetServiceProvider(sp);
            return configurator.Receiver.ReceiverListenerContainer;
        }

        private static ISubscriberContextContainer TryResolveSubscriberContextContainer(IServiceProvider sp,
            IServiceBusClientConfigurator configurator)
        {
            foreach (var (topicName, consumerContext) in configurator.Receiver.ReceiverContextContainer.Subscriber
                         .Contexts)
            {
                var receiverListener = CreateReceiverListener(sp, consumerContext);
                configurator.Receiver.ReceiverListenerContainer.AddSubscriber(topicName, receiverListener);
            }

            return configurator.Receiver.ReceiverContextContainer.Subscriber;
        }

        private static ReceiverListener CreateReceiverListener(IServiceProvider sp, SubscriberContext consumerContext)
        {
            var receiverListenerLogger = sp.GetRequiredService<ILogger<ReceiverListener>>();

            var serviceBusClient = sp.GetRequiredService<ServiceBusClient>();
            var serviceBusAdministrationClient = sp.GetRequiredService<ServiceBusAdministrationClient>();

            IServiceBusHostSettings hostSettings =
                new ServiceBusHostSettings("", serviceBusClient, serviceBusAdministrationClient);

            var receiverListener = new ReceiverListener(receiverListenerLogger, hostSettings, consumerContext);
            receiverListener.ConnectObservers(sp);

            return receiverListener;
        }
    }
}