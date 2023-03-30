namespace Rydo.AzureServiceBus.Client.Consumers.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CSharpFunctionalExtensions;
    using Handlers;
    using Topics;

    internal static class CustomHandlerEx
    {
        public static Result<(Type ContractType, Type HandlerType)> TryExtractCustomerHandlers(this IEnumerable<Type> types, string topicName)
        {
            var enumerableTypes = types as Type[] ?? types.ToArray();

            var consumerHandlers =
                new Dictionary<(string, string), (Type ContractType, Type HandlerType)>();

            var assemblies = enumerableTypes.Select(x => x.Assembly);
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

            return clientTypes.Value.HandlerType == null
                ? Result.Failure<(Type ContractType, Type HandlerType)>($"No Custom Handler found for topic {topicName}")
                : Result.Success(clientTypes.Value);
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
    }
}