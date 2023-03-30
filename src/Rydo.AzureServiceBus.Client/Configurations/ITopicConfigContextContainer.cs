namespace Rydo.AzureServiceBus.Client.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    public interface ITopicConfigContextContainer
    {
        ImmutableDictionary<string, TopicDefinition> Entries { get; }

        void AddTopicConfig(IEnumerable<TopicConfig> topicConfigs, DeadLetterPolicyEntry deadLetterPolicyEntry,
            Type type);
    }

    public sealed class TopicConfigContextContainer : ITopicConfigContextContainer
    {
        public TopicConfigContextContainer()
        {
            Entries = ImmutableDictionary<string, TopicDefinition>.Empty;
        }

        public ImmutableDictionary<string, TopicDefinition> Entries { get; private set; }

        public void AddTopicConfig(IEnumerable<TopicConfig> topicConfigs, DeadLetterPolicyEntry deadLetterPolicyEntry,
            Type type)
        {
            foreach (var topicConfig in topicConfigs)
            {
                TryAddTopicConfig(topicConfig, deadLetterPolicyEntry, type);
            }
        }

        private void TryAddTopicConfig(TopicConfig topicConfig, DeadLetterPolicyEntry deadLetterPolicyEntry, Type type)
        {
            if (string.IsNullOrEmpty(topicConfig.Name))
                throw new ArgumentException("topicConfig.Name");

            TopicDefinition topicDefinition;
            
            if (HasConfigForDeadLetter(topicConfig, deadLetterPolicyEntry))
            {
                if (deadLetterPolicyEntry.Entries.TryGetValue(topicConfig.DeadLetterPolicyName,
                        out var deadLetterPolicyItem))
                {
                    topicDefinition = topicConfig.AdapterConfigToDefinition(deadLetterPolicyItem, type);
                    Entries = Entries.Add(topicDefinition.TopicName, topicDefinition);

                    return;
                }
            }
            
            topicDefinition = topicConfig.AdapterConfigToDefinition(type);
            if (topicDefinition is null)
                throw new ArgumentException("topicConfig.Name");


            Entries = Entries.Add(topicDefinition.TopicName, topicDefinition);
        }
        
        private static bool HasConfigForDeadLetter(TopicConfig topicConfig, DeadLetterPolicyEntry deadLetterPolicyEntry) =>
            !topicConfig.Direction.Equals(TopicDirections.Producer)
            && !string.IsNullOrEmpty(topicConfig.DeadLetterPolicyName)
            && deadLetterPolicyEntry.Entries.Any();
    }
}