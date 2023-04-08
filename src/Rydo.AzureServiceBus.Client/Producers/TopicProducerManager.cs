namespace Rydo.AzureServiceBus.Client.Producers
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Exceptions;
    using Microsoft.Extensions.Logging;
    using Topics;

    public sealed class TopicProducerManager : ITopicProducerManager
    {
        private readonly object _lockObject;
        private readonly ILogger<TopicProducerManager> _logger;

        private ImmutableDictionary<string, string> _cache;

        public TopicProducerManager(ILogger<TopicProducerManager> logger)
        {
            _logger = logger;
            _lockObject = new object();
            _cache = ImmutableDictionary<string, string>.Empty;
        }

        public bool TryExtractTopicName(object model, out string topicName) =>
            InternalTryExtractTopicName(model, out topicName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool InternalTryExtractTopicName(object model, out string topicName)
        {
            topicName = string.Empty;

            if (model == null)
                return false;

            var modeFullName = model.GetType().FullName;

            lock (_lockObject)
            {
                if (_cache.TryGetValue(modeFullName, out topicName))
                    return true;
            }

            lock (_lockObject)
            {
                if (!model.TryExtractTopicName(out topicName))
                {
                    topicName = string.Empty;
                    return false;
                }

                if (string.IsNullOrWhiteSpace(topicName))
                {
                    throw new InvalidTopicNameException(modeFullName);
                }

                _cache = _cache.Add(modeFullName, topicName);
            }

            return true;
        }
    }

    internal static class ModelExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryExtractTopicName(this object model, out string topicName)
        {
            topicName = string.Empty;

            if (model == null)
                return false;

            if (!model.GetType().CustomAttributes.Any())
            {
                topicName = string.Empty;
                return false;
            }

            var attr = model.GetType().CustomAttributes
                .SingleOrDefault(x =>
                    x.AttributeType.FullName is TopicProducerAttribute.FullNameTopicProducerAttribute);

            if (attr == null)
            {
                topicName = string.Empty;
                return false;
            }

            topicName = attr.ConstructorArguments[TopicProducerAttribute.TopicNamePosition].Value.ToString();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryExtractTopicName(this Type model, out string topicName)
        {
            topicName = string.Empty;

            if (model == null)
                return false;

            if (!model.CustomAttributes.Any())
            {
                topicName = string.Empty;
                return false;
            }

            var attr = model.CustomAttributes
                .SingleOrDefault(x =>
                    x.AttributeType.FullName is TopicConsumerAttribute.FullNameTopicConsumerAttribute);

            if (attr == null)
            {
                topicName = string.Empty;
                return false;
            }

            topicName = attr.ConstructorArguments[TopicConsumerAttribute.TopicNamePosition].Value.ToString();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryExtractContractType(this Type model, out Type contractType)
        {
            contractType = default;

            if (model == null)
                return false;

            if (!model.CustomAttributes.Any())
                return false;

            var attr = model.CustomAttributes
                .SingleOrDefault(x =>
                    x.AttributeType.FullName is TopicConsumerAttribute.FullNameTopicConsumerAttribute);

            if (attr == null)
                return false;

            contractType = (Type)attr.ConstructorArguments[TopicConsumerAttribute.ContractTypeNamePosition].Value;
            return true;
        }
    }
}