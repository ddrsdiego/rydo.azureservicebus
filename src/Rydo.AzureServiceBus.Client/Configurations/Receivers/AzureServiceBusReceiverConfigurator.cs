namespace Rydo.AzureServiceBus.Client.Configurations.Receivers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Client.Extensions;
    using Consumers.Subscribers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
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
                    var subscriber = new ReceiverListener(consumerContext);
                    _receiverListenerContainer.AddSubscriber(topicName, subscriber);
                }

                return _receiverContextContainer.Subscriber;
            };
    }
}