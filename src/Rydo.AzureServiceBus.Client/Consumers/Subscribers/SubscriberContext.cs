namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;

    public sealed class SubscriberContext
    {
        public SubscriberContext(SubscriberSpecification subscriberSpecification, Type contractType, Type handlerType)
        {
            ContractType = contractType;
            HandlerType = handlerType;
            SubscriberSpecification = subscriberSpecification;
        }

        public readonly Type HandlerType;
        public readonly Type ContractType;
        public readonly SubscriberSpecification SubscriberSpecification;
    }
}