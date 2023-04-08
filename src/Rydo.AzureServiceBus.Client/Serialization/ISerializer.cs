namespace Rydo.AzureServiceBus.Client.Serialization
{
    public interface ISerializer:
        IMessageSerializer,
        IMessageDeserializer,
        IObjectDeserializer
    {
    }
}