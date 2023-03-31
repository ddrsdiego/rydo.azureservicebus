namespace Rydo.AzureServiceBus.Client.Configurations.Extensions
{
    using System;

    internal static class TopicConfigAdapterExtension
    {
        public static TopicDefinition AdapterConfigToDefinition(this TopicConfig topicConfig, Type type)
        {
            var deadLetterPolicyItem = new DeadLetterPolicyItem {Retry = DeadLetterPolicyItem.EmptyRetry};
            return topicConfig.AdapterConfigToDefinition(deadLetterPolicyItem, type);
        }

        public static TopicDefinition AdapterConfigToDefinition(this TopicConfig topicConfig,
            DeadLetterPolicyItem deadLetterPolicyItem, Type type)
        {
            var subscriptionName = string.Empty;

            if (!topicConfig.Direction.Equals(TopicDirections.Producer, StringComparison.InvariantCultureIgnoreCase))
            {
                subscriptionName = string.IsNullOrEmpty(topicConfig.SubscriptionName)
                    ? GetConsumerGroupDefault(topicConfig, type)
                    : topicConfig.SubscriptionName;
            }

            TopicDefinition topicDefinition = topicConfig.Direction switch
            {
                TopicDirections.Producer => new TopicProducerDefinition(topicConfig.Name),
                TopicDirections.Consumer => new TopicConsumerDefinition(topicConfig.Name, subscriptionName),
                TopicDirections.Both => new TopicBothDefinition(topicConfig.Name, subscriptionName),
                _ => throw new ArgumentOutOfRangeException()
            };

            return topicDefinition;
        }

        private static string GetConsumerGroupDefault(TopicConfig topicConfig, Type type)
        {
            var subscriptionNamePrefix = type?.Assembly.GetName().Name?.ToLowerInvariant();

            var consumerGroupDefault = $"{subscriptionNamePrefix}-{topicConfig.Name.ToUpperInvariant()}";
            return consumerGroupDefault;
        }
    }
}