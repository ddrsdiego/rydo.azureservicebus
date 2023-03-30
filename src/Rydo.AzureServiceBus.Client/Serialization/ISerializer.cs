namespace Rydo.AzureServiceBus.Client.Serialization
{
    using System.Threading.Tasks;

    public interface ISerializer
    {
        byte[] Serialize<T>(T obj);
        
        T Deserialize<T>(byte[] data);
        
        ValueTask<byte[]> SerializeAsync<T>(T obj);
        
        ValueTask<T> DeserializeAsync<T>(byte[] data);
    }
}