namespace Rydo.AzureServiceBus.Client.Serialization
{
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    internal sealed class SystemTextJsonSerializer : ISerializer
    {
        private readonly ILogger<SystemTextJsonSerializer> _logger;
        private readonly JsonSerializerOptions _options;

        public SystemTextJsonSerializer(ILogger<SystemTextJsonSerializer> logger, JsonSerializerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        public byte[] Serialize<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj, _options);
        }

        public T Deserialize<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data, _options);
        }

        public async ValueTask<byte[]> SerializeAsync<T>(T obj)
        {
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, obj, _options);

            return stream.ToArray();
        }

        public async ValueTask<T> DeserializeAsync<T>(byte[] data)
        {
            using var stream = new MemoryStream(data);
            return await JsonSerializer.DeserializeAsync<T>(stream, _options);
        }
    }
}