namespace Rydo.AzureServiceBus.Client.Producers
{
    using Azure.Messaging.ServiceBus;

    public sealed class ProducerContext
    {
        public ProducerContext(ProducerSpecification producerSpecification, ServiceBusSender sender)
        {
            ProducerSpecification = producerSpecification;
            Sender = sender;
        }

        public readonly ProducerSpecification ProducerSpecification;
        public ServiceBusSender Sender { get; }
    }
}