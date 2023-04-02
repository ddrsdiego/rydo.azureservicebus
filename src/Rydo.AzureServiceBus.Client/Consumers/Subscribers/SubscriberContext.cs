namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;

    public sealed class SubscriberContext
    {
        public SubscriberContext(SubscriberSpecification specification, Type contractType, Type handlerType)
        {
            ContractType = contractType;
            HandlerType = handlerType;
            Specification = specification;
        }

        public readonly Type HandlerType;
        public readonly Type ContractType;
        public readonly SubscriberSpecification Specification;

        public string TopicSubscriptionName => $"{Specification.SubscriptionName}-{Specification.TopicName}";
    }
}