namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;

    public interface ISubscriberContext
    {
    }

    public sealed class SubscriberContext :
        ISubscriberContext
    {
        internal SubscriberContext(SubscriberSpecification specification, Type contractType, Type handlerType)
        {
            ContractType = contractType;
            HandlerType = handlerType;
            Specification = specification;
        }

        internal readonly Type HandlerType;
        internal readonly Type ContractType;
        internal readonly SubscriberSpecification Specification;
    }
}