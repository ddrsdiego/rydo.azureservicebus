namespace Rydo.AzureServiceBus.Client.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Consumers;
    using Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Middlewares.Extensions;
    using Services;
    using Subscribers;

    internal sealed class AzureServiceBusConsumerConfigurator : IAzureServiceBusConsumerConfigurator
    {
        private readonly IServiceCollection _services;
        private readonly ISubscriberContainer _subscriberContainer;
        private readonly IConsumerContextContainer _consumerContextContainer;

        public AzureServiceBusConsumerConfigurator(IServiceCollection services)
        {
            _services = services;
            _subscriberContainer = new SubscriberContainer();
            _consumerContextContainer = new ConsumerContextContainer(_services);
        }

        public void Configure(Type type, Action<IConsumerContextContainer> container) =>
            Configure(new List<Type> {type}, container);

        public void Configure(IEnumerable<Type> types, Action<IConsumerContextContainer> container)
        {
            var enumerableTypes = types as Type[] ?? types.ToArray();
            
            _consumerContextContainer.WithTypes(enumerableTypes);
            container(_consumerContextContainer);
            
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

        private Func<IServiceProvider, IConsumerContextContainer> ConfigureConsumerContextContainer()
        {
            return sp =>
            {
                foreach (var (topicName, consumerContext) in _consumerContextContainer.Contexts)
                {
                    var subscriber = new Subscriber(consumerContext);
                    _subscriberContainer.AddSubscriber(topicName, subscriber);
                }

                return _consumerContextContainer;
            };
        }
    }
}