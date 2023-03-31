namespace Rydo.AzureServiceBus.Client.Configurations.Receivers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions.Observers.Observables;
    using Client.Extensions;
    using Consumers.Subscribers;
    using Logging.Observers;
    using Metrics.Observers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Middlewares.Extensions;
    using Services;

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

        public void Configure(Type type, Action<IReceiverContextContainer> container)
        {
            var queueName = type?.Assembly.GetName().Name?.ToLowerInvariant();
            Configure(new List<Type> {type}, queueName, container);
        }

        public void Configure(Type type, string queueName, Action<IReceiverContextContainer> container)
        {
        }

        public void Configure(IEnumerable<Type> types, Action<IReceiverContextContainer> container)
        {
            throw new NotImplementedException();
        }

        public void Configure(IEnumerable<Type> types, string queueName, Action<IReceiverContextContainer> container)
        {
            var enumerableTypes = types as Type[] ?? types.ToArray();

            _receiverContextContainer
                .Subscriber
                .WithTypes(enumerableTypes);

            container(_receiverContextContainer);

            foreach (var handlerType in enumerableTypes.GetHandlerTypes())
                _services.TryAddScoped(handlerType ?? throw new InvalidOperationException());

            _services.AddMiddlewares();
            _services.AddSingleton(ConfigureConsumerContextContainer());
            _services.AddSingleton(sp =>
            {
                _receiverListenerContainer.SetServiceProvider(sp);
                return _receiverListenerContainer;
            });

            _services.TryAddSingleton(_receiverContextContainer);
            _services.AddHostedService<AzureServiceBusIntegrationHostedService>();
        }

        private Func<IServiceProvider, ISubscriberContextContainer> ConfigureConsumerContextContainer() =>
            sp =>
            {
                foreach (var (topicName, consumerContext) in _receiverContextContainer.Subscriber.Contexts)
                {
                    var receiverListener = CreateReceiverListener(sp, consumerContext);
                    ConnectObservers(sp, receiverListener);

                    _receiverListenerContainer.AddSubscriber(topicName, receiverListener);
                }

                return _receiverContextContainer.Subscriber;
            };

        private static void ConnectObservers(IServiceProvider sp, IReceiverListener receiverListener)
        {
            var logLoggingReceiveObserver = sp.GetRequiredService<ILogger<LoggingReceiveObserver>>();
            receiverListener.ConnectReceiveObserver(new LoggingReceiveObserver(logLoggingReceiveObserver));
        }

        private static ReceiverListener CreateReceiverListener(IServiceProvider sp, SubscriberContext consumerContext)
        {
            var receiverListenerLogger = sp.GetRequiredService<ILogger<ReceiverListener>>();
            var receiverListener = new ReceiverListener(receiverListenerLogger, consumerContext);
            return receiverListener;
        }
    }
}