namespace Rydo.AzureServiceBus.Client.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
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
            if (string.IsNullOrWhiteSpace(topicName)) throw new ArgumentNullException(nameof(topicName));

            if (Contexts.TryGetValue(topicName, out var _))
                throw new InvalidOperationException(nameof(topicName));

            var consumerHandlers =
                new Dictionary<(string, string), (Type ContractType, Type HandlerType)>();
            
            var assemblies = _types.Select(x => x.Assembly);
            foreach (var exportedTypes in assemblies.Select(x => x.ExportedTypes))
            {
                foreach (var exportedType in exportedTypes)
                {
                    if (!TryGetConsumerHandler(exportedType, out var consumerHandlerType))
                        continue;

                    (string, string) consumerHandlerId = (exportedType.Assembly.GetName().Name, exportedType.FullName);
                    consumerHandlers.Add(consumerHandlerId, consumerHandlerType);
                }
            }

            var clientTypes = consumerHandlers
                .FirstOrDefault(messageHandler => IsTopicConsumerAttribute(messageHandler, topicName));

            if (clientTypes.Value.HandlerType == null)
                return;

            var builder = new ConsumerConfiguratorBuilder(topicName);
            configurator(builder);

            var consumerConfigurator = !builder.HasBuild
                ? builder.Build()
                : builder.ConsumerConfigurator;

            var subscriptionName = string.IsNullOrWhiteSpace(consumerConfigurator.SubscriptionName)
                ? GetSubscriptionName(consumerConfigurator, _types.First())
                : consumerConfigurator.SubscriptionName;

            var consumerSpecification = new ConsumerSpecification(topicName, subscriptionName, 10,
                consumerConfigurator.LockDurationInMinutes, consumerConfigurator.MaxDeliveryCount);

            var context = new ConsumerContext(consumerSpecification, clientTypes.Value.ContractType,
                clientTypes.Value.HandlerType);

            Contexts = Contexts.Add(context.ConsumerSpecification.TopicName, context);
        }

        private static bool TryGetConsumerHandler(Type type,
            out (Type ContractType, Type HandlerType) consumerHandlerType)
        {
            consumerHandlerType = default;
            if (!type.GetInterfaces().Any(FindConsumerHandler))
                return false;

            var clientContract = type
                .GetInterfaces()
                .Where(x => x.IsGenericType && typeof(IConsumerHandler).IsAssignableFrom(x))
                .Select(x => x.GenericTypeArguments[0]).FirstOrDefault();

            consumerHandlerType = (clientContract, type);
            return true;
        }

        private static bool IsTopicConsumerAttribute(
            KeyValuePair<(string, string), (Type MessageContractType, Type MessageHandlerType)> clientTypes,
            string topicName)
        {
            var (key, (messageContractType, messageHandlerType)) = clientTypes;
            if (messageHandlerType is null)
                throw new Exception();

            return messageHandlerType
                .CustomAttributes.FirstOrDefault(x => x.AttributeType.Name.Contains(nameof(TopicConsumerAttribute)))
                .ConstructorArguments.Any(x =>
                    x.Value != null &&
                    x.Value.ToString().Equals(topicName, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool FindConsumerHandler(Type type)
        {
            return type.IsInterface
                   && !type.IsGenericType
                   && type.GenericTypeArguments.Length == 0
                   && type.Name.Equals(nameof(IConsumerHandler), StringComparison.InvariantCultureIgnoreCase);
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