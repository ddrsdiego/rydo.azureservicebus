namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Producers;

    internal sealed class SubscriberContextContainer : ISubscriberContextContainer
    {
        private IEnumerable<Type> _types;
        private Type _consumerHandler;
        private readonly IServiceCollection _services;

        public SubscriberContextContainer(IServiceCollection services)
        {
            _services = services;
            Contexts = ImmutableDictionary<string, SubscriberContext>.Empty;
        }

        public void WithTypes(IEnumerable<Type> types)
        {
            _types = types ?? throw new ArgumentNullException(nameof(types));
        }

        public void WithConsumerHandler<T>() => _consumerHandler = typeof(T);

        public void Add()
        {
            var subscriptionName = _consumerHandler.Assembly.GetName().Name.ToLowerInvariant();
            Add(subscriptionName, subscriber => subscriber.Build());
        }

        public void Add(string subscriptionName) => Add(subscriptionName, subscriber => subscriber.Build());

        public void Add(Action<SubscriberConfiguratorBuilder> configurator)
        {
            var subscriptionName = _consumerHandler.Assembly.GetName().Name.ToLowerInvariant();
            Add(subscriptionName, configurator);
        }

        public void Add(string subscriptionName, Action<SubscriberConfiguratorBuilder> configurator)
        {
            _consumerHandler.TryExtractTopicNameFromConsumer(out var topicOrQueueName);

            var result = new List<Type> {_consumerHandler}.TryExtractCustomerHandlers(topicOrQueueName);
            if (result.IsFailure)
                return;

            var builder = new SubscriberConfiguratorBuilder(topicOrQueueName, subscriptionName);
            configurator(builder);

            var consumerConfigurator = !builder.HasBuild
                ? builder.Build().Value
                : builder.ConsumerConfigurator;

            if (string.IsNullOrEmpty(subscriptionName))
            {
                subscriptionName = string.IsNullOrWhiteSpace(consumerConfigurator.SubscriptionName)
                    ? GetSubscriptionName(consumerConfigurator, _types.First())
                    : consumerConfigurator.SubscriptionName;
            }

            var consumerSpecification = new SubscriberSpecification(consumerConfigurator);
            var context = new SubscriberContext(consumerSpecification, result.Value.ContractType, _consumerHandler);
            Contexts = Contexts.Add(context.TopicSubscriptionName, context);
        }

        public ImmutableDictionary<string, SubscriberContext> Contexts { get; private set; }

        public bool TryGetConsumerContext(string topicName, out SubscriberContext context)
        {
            context = default;
            return true;
        }

        private static string GetSubscriptionName(IConsumerConfigurator consumerConfigurator, Type type)
        {
            var subscriptionNamePrefix = type?.Assembly.GetName().Name?.ToLowerInvariant();
            return type?.Assembly.GetName().Name?.ToLowerInvariant();
        }
    }
}