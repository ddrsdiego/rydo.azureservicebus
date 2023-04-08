namespace Rydo.AzureServiceBus.Client.Serialization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IObjectDeserializer
    {
        ValueTask<object> DeserializeAsync(byte[] data, Type type, CancellationToken cancellationToken = default);
    }
}