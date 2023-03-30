namespace Rydo.AzureServiceBus.Client.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Extensions;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Topics;

    internal sealed class ConsumerContextContainer : IConsumerContextContainer
    {
        private IEnumerable<Type> _types;
        private readonly IServiceCollection _services;

        public ConsumerContextContainer(IServiceCollection services)
        {
            _services = services;
            Contexts = ImmutableDictionary<string, ConsumerContext>.Empty;
        }

        public void WithTypes(IEnumerable<Type> types)
        {
            _types = types ?? throw new ArgumentNullException(nameof(types));
        }

        public ImmutableDictionary<string, ConsumerContext> Contexts { get; private set; }

        public void AddSubscriber(string topicName) => AddSubscriber(topicName, configurator => configurator.Build());

        public void AddSubscriber(string topicName, Action<ConsumerConfiguratorBuilder> configurator)
        {
            if (Contexts.TryGetValue(topicName, out var _)) throw new InvalidOperationException(nameof(topicName));
            
            var result = _types.TryExtractCustomerHandlers(topicName);
            if (result.IsFailure)
                return;

            var builder = new ConsumerConfiguratorBuilder(topicName);
            configurator(builder);

            var consumerConfigurator = !builder.HasBuild
                ? builder.Build().Value
                : builder.ConsumerConfigurator;

            var subscriptionName = string.IsNullOrWhiteSpace(consumerConfigurator.SubscriptionName)
                ? GetSubscriptionName(consumerConfigurator, _types.First())
                : consumerConfigurator.SubscriptionName;

            var consumerSpecification = new ConsumerSpecification(topicName, subscriptionName,
                consumerConfigurator.MaxMessages,
                consumerConfigurator.LockDurationInMinutes,
                consumerConfigurator.MaxDeliveryCount);

            var context = new ConsumerContext(consumerSpecification, result.Value.ContractType,
                result.Value.HandlerType);

            Contexts = Contexts.Add(context.ConsumerSpecification.TopicName, context);
        }

        public bool TryGetConsumerContext(string topicName, out ConsumerContext context)
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