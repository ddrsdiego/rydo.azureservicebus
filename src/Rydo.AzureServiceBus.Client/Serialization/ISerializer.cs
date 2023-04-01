namespace Rydo.AzureServiceBus.Client.Serialization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISerializer
    {
        byte[] Serialize<T>(T obj);

        T Deserialize<T>(byte[] data);

        ValueTask<byte[]> SerializeAsync<T>(T obj, CancellationToken cancellationToken = default);

        ValueTask<T> DeserializeAsync<T>(byte[] data, CancellationToken cancellationToken = default);
        
        ValueTask<object> DeserializeAsync(byte[] data, Type type, CancellationToken cancellationToken = default);
    }
}