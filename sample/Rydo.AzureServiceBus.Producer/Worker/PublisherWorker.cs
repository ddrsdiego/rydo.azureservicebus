namespace Rydo.AzureServiceBus.Producer.Worker
{
    using System.Net.Mime;
    using System.Reflection;
    using System.Text.Json;
    using Azure.Messaging.ServiceBus;

    internal sealed class PublisherWorker : BackgroundService
    {
        private readonly ServiceBusClient _serviceBusClient;

        public PublisherWorker(ServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int capacity = 10_000;
            while (!stoppingToken.IsCancellationRequested)
            {
                var sender = _serviceBusClient.CreateSender(TopicNameConstants.AccountCreated);
                var tasks = new List<Task>(capacity);
                for (var index = 1; index <= capacity; index++)
                {
                    var accountNumber = index.ToString("0000000");

                    var accountCreatedMessage = new ConsumerHandlers.AccountCreated(accountNumber);

                    var producerName = Assembly.GetExecutingAssembly().GetName().Name?.ToLowerInvariant();
                    var payload = JsonSerializer.SerializeToUtf8Bytes(accountCreatedMessage);

                    var message = new ServiceBusMessage(payload)
                    {
                        ContentType = MediaTypeNames.Application.Json,
                        ApplicationProperties =
                        {
                            new KeyValuePair<string, object>("producer", producerName),
                        },
                        PartitionKey = accountCreatedMessage.AccountNumber
                    };

                    tasks.Add(sender.SendMessageAsync(message, stoppingToken));
                }

                await Task.WhenAll(tasks);
                await Task.Delay(10_000, stoppingToken);
            }
        }
    }
}