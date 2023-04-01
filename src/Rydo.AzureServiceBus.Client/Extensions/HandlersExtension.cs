namespace Rydo.AzureServiceBus.Client.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Handlers;

    internal static class Ex
    {
        public static Type GetConsumerHandler(this Type types)
        {
            Type handlerTypes = default;

            foreach (var exportedType in types.Assembly.ExportedTypes)
            {
                if (!TryGetConsumerHandler(exportedType, out var consumerHandlerType))
                    continue;

                handlerTypes = consumerHandlerType.HandlerType;
            }

            return handlerTypes;
        }

        public static IEnumerable<Type> GetHandlerTypes(this IEnumerable<Type> types)
        {
            var handlerTypes = new List<Type>();

            foreach (var assembly in types.Select(x => x.Assembly))
            {
                foreach (var exportedType in assembly.ExportedTypes)
                {
                    if (!TryGetConsumerHandler(exportedType, out var consumerHandlerType))
                        continue;

                    handlerTypes.Add(consumerHandlerType.HandlerType ?? throw new InvalidOperationException());
                }
            }

            return handlerTypes;
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

            static bool FindConsumerHandler(Type type) =>
                type.IsInterface
                && !type.IsGenericType
                && type.GenericTypeArguments.Length == 0
                && type.Name.Equals(nameof(IConsumerHandler), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}