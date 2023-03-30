namespace Rydo.AzureServiceBus.Client.Producers
{
    using System.Text.Json;

    public interface IProducerConfigurator
    {
        JsonSerializerOptions Options { get; set; }
    }

    internal sealed class ProducerConfigurator : IProducerConfigurator
    {
        public JsonSerializerOptions Options { get; set; }
    }
}