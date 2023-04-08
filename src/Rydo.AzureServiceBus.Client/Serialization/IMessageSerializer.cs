namespace Rydo.AzureServiceBus.Client.Serialization
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMessageSerializer
    {
        byte[] Serialize<T>(T obj);
        
        ValueTask<byte[]> SerializeAsync<T>(T obj, CancellationToken cancellationToken = default);
    }
}