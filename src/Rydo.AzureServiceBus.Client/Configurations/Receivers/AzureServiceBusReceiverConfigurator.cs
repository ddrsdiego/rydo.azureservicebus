namespace Rydo.AzureServiceBus.Client.Configurations.Receivers
{
    using System;
    using Client.Producers;
    using Consumers.Subscribers;
    using Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;

    internal sealed class AzureServiceBusReceiverConfigurator : IAzureServiceBusReceiverConfigurator
    {
        private readonly IServiceCollection _services;
        private readonly IReceiverListenerContainer _receiverListenerContainer;
        private readonly IReceiverContextContainer _receiverContextContainer;

        public AzureServiceBusReceiverConfigurator(IServiceCollection services)
        {
            _services = services;
            _receiverListenerContainer = new ReceiverListenerContainer();
            _receiverContextContainer = new ReceiverContextContainer(services);
        }

        public void Configure<T>() => Configure<T>(c => c.Subscriber.Add());

        public void Configure<T>(Action<IReceiverContextContainer> container)
        {
            if (!typeof(T).TryExtractTopicNameFromConsumer(out var topicName))
                throw new InvalidOperationException("");

            _receiverContextContainer.Subscriber.WithConsumerHandler<T>();
            container(_receiverContextContainer);

            _services.TryAddScoped(typeof(T));
            _services.TryAddSingleton(_receiverContextContainer);
            _services.TryAddSingleton(TryResolveReceiverListenerContainer);
            _services.TryAddSingleton(TryResolveSubscriberContextContainer);
        }

        private IReceiverListenerContainer TryResolveReceiverListenerContainer(IServiceProvider sp)
        {
            _receiverListenerContainer.SetServiceProvider(sp);
            return _receiverListenerContainer;
        }

        private ISubscriberContextContainer TryResolveSubscriberContextContainer(IServiceProvider sp)
        {
            foreach (var (topicName, consumerContext) in _receiverContextContainer.Subscriber.Contexts)
            {
                var receiverListener = CreateReceiverListener(sp, consumerContext);
                _receiverListenerContainer.AddSubscriber(topicName, receiverListener);
            }

            return _receiverContextContainer.Subscriber;
        }

        private static ReceiverListener CreateReceiverListener(IServiceProvider sp, SubscriberContext consumerContext)
        {
            var receiverListenerLogger = sp.GetRequiredService<ILogger<ReceiverListener>>();
            var receiverListener = new ReceiverListener(receiverListenerLogger, consumerContext);

            receiverListener.ConnectObservers(sp);
            return receiverListener;
        }
    }
}