namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Extensions;

    internal sealed class SubscriberContextContainer : ISubscriberContextContainer
    {
        private IEnumerable<Type> _types;
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

        public ImmutableDictionary<string, SubscriberContext> Contexts { get; private set; }

        public void AddSubscriber(string topicName) => AddSubscriber(topicName, configurator => configurator.Build());

        public void AddSubscriber(string topicName, string subscriptionName) =>
            AddSubscriber(topicName, string.Empty, configurator => configurator.Build());

        public void AddSubscriber(string topicName, Action<SubscriberConfiguratorBuilder> configurator) =>
            AddSubscriber(topicName, string.Empty, configurator);

        public void AddSubscriber(string topicName, string subscriptionName,
            Action<SubscriberConfiguratorBuilder> configurator)
        {
            if (Contexts.TryGetValue(topicName, out var _)) throw new InvalidOperationException(nameof(topicName));

            var result = _types.TryExtractCustomerHandlers(topicName);
            if (result.IsFailure)
                return;

            var builder = new SubscriberConfiguratorBuilder(topicName);
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

            var consumerSpecification = new SubscriberSpecification(topicName, subscriptionName,
                consumerConfigurator.MaxMessages,
                consumerConfigurator.LockDurationInMinutes,
                consumerConfigurator.MaxDeliveryCount);

            var context = new SubscriberContext(consumerSpecification, result.Value.ContractType,
                result.Value.HandlerType);

            Contexts = Contexts.Add(context.SubscriberSpecification.TopicName, context);
        }

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