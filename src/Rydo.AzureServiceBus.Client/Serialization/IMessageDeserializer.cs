namespace Rydo.AzureServiceBus.Client.Serialization
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMessageDeserializer
    {
        T Deserialize<T>(byte[] data);
        
        ValueTask<T> DeserializeAsync<T>(byte[] data, CancellationToken cancellationToken = default);
    }
}