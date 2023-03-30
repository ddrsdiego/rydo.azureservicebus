namespace Rydo.AzureServiceBus.Client.Producers.Extensions
{
    using System.Net.Mime;
    using System.Text.Json;
    using Azure.Messaging.ServiceBus;
    using Headers;
    using Microsoft.Extensions.Logging;

    public interface IMessageFactory<out TMessage>
    {
        TMessage To(ProducerRequest request);
    }
    
    internal sealed class ServiceBusMessageFactory : IMessageFactory<ServiceBusMessage>
    {
        private readonly ILogger<ServiceBusMessageFactory> _logger;

        public ServiceBusMessageFactory(ILogger<ServiceBusMessageFactory> logger)
        {
            _logger = logger;
        }
        
        public ServiceBusMessage To(ProducerRequest request)
        {
            return request.ToServiceBusMessage();
        }
    }

    public static class ProducerRequestEx
    {
        public static ServiceBusMessage ToServiceBusMessage(this ProducerRequest request)
        {
            var payload = JsonSerializer.SerializeToUtf8Bytes(request.Message);

            var partitionKey = request.MessageHeaders.GetString(MessageHeadersDefault.PartitionKey);
            var message = new ServiceBusMessage(payload)
            {
                ContentType = MediaTypeNames.Application.Json,
                PartitionKey = partitionKey
            };

            return message;
        }
    }
}