namespace Rydo.AzureServiceBus.Client.Consumers
{
    using System;

    public sealed class ConsumerContext
    {
        public ConsumerContext(ConsumerSpecification consumerSpecification, Type contractType, Type handlerType)
        {
            ContractType = contractType;
            HandlerType = handlerType;
            ConsumerSpecification = consumerSpecification;
        }

        public readonly Type HandlerType;
        public readonly Type ContractType;
        public readonly ConsumerSpecification ConsumerSpecification;
    }
}