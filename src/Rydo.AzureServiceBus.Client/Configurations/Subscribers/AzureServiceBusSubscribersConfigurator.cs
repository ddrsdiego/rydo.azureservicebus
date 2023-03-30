namespace Rydo.AzureServiceBus.Client.Configurations.Subscribers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Rydo.AzureServiceBus.Client.Consumers.Subscribers;
    using Rydo.AzureServiceBus.Client.Extensions;
    using Rydo.AzureServiceBus.Client.Middlewares.Extensions;
    using Services;

    internal sealed class AzureServiceBusSubscribersConfigurator : IAzureServiceBusSubscribersConfigurator
    {
        private readonly IServiceCollection _services;
        private readonly ISubscriberContainer _subscriberContainer;
        private readonly ISubscriberContextContainer _subscriberContextContainer;

        public AzureServiceBusSubscribersConfigurator(IServiceCollection services)
        {
            _services = services;
            _subscriberContainer = new SubscriberContainer();
            _subscriberContextContainer = new SubscriberContextContainer(_services);
        }

        public void Configure(Type type, Action<ISubscriberContextContainer> container) =>
            Configure(new List<Type> {type}, container);

        public void Configure(IEnumerable<Type> types, Action<ISubscriberContextContainer> container)
        {
            var enumerableTypes = types as Type[] ?? types.ToArray();
            
            _subscriberContextContainer.WithTypes(enumerableTypes);
            container(_subscriberContextContainer);
            
            foreach (var handlerType in enumerableTypes.GetHandlerTypes())
                _services.TryAddScoped(handlerType ?? throw new InvalidOperationException());
            
            _services.AddMiddlewares();
            _services.AddSingleton(ConfigureConsumerContextContainer());
            _services.AddSingleton(serviceProvider =>
            {
                _subscriberContainer.SetServiceProvider(serviceProvider);
                return _subscriberContainer;
            });

            _services.AddHostedService<AzureServiceBusIntegrationHostedService>();
        }

        private Func<IServiceProvider, ISubscriberContextContainer> ConfigureConsumerContextContainer()
        {
            return sp =>
            {
                foreach (var (topicName, consumerContext) in _subscriberContextContainer.Contexts)
                {
                    var subscriber = new Subscriber(consumerContext);
                    _subscriberContainer.AddSubscriber(topicName, subscriber);
                }

                return _subscriberContextContainer;
            };
        }
    }
}