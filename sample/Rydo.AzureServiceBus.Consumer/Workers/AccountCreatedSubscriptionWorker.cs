namespace Rydo.AzureServiceBus.Consumer.Workers
{
    using System.Text.Json;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;

    internal sealed class AccountCreatedSubscriptionWorker : BackgroundService
    {
        private const string SubscriptionName = "rydo.azureservicebus.consumer";
        private const string TopicName = "rydo-azureservicebus-account-created";

        private ServiceBusReceiver? _receiver;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusAdministrationClient _administrationClient;
        private readonly ILogger<AccountCreatedSubscriptionWorker> _logger;

        public AccountCreatedSubscriptionWorker(ILogger<AccountCreatedSubscriptionWorker> logger, ServiceBusClient serviceBusClient,
            ServiceBusAdministrationClient administrationClient)
        {
            _logger = logger;
            _serviceBusClient = serviceBusClient;
            _administrationClient = administrationClient;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitiateEntities(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        private async Task InitiateEntities(CancellationToken cancellationToken)
        {
            await InitiateTopics(cancellationToken);
            await InitiateSubscriptions(cancellationToken);
        }

        private async Task InitiateSubscriptions(CancellationToken cancellationToken)
        {
            var subscriptionExists =
                await _administrationClient.SubscriptionExistsAsync(TopicName, SubscriptionName, cancellationToken);
            if (!subscriptionExists.Value)
            {
                var subscriptionOptions =
                    new CreateSubscriptionOptions(TopicName, SubscriptionName)
                    {
                        LockDuration = TimeSpan.FromMinutes(1),
                        MaxDeliveryCount = 5
                    };

                await _administrationClient.CreateSubscriptionAsync(subscriptionOptions, cancellationToken);
            }
        }

        private async Task InitiateTopics(CancellationToken cancellationToken)
        {
            var topicExistsAsync = await _administrationClient.TopicExistsAsync(TopicName, cancellationToken);
            if (!topicExistsAsync.Value)
            {
                var createQueueOptions = new CreateTopicOptions(TopicName)
                {
                    EnablePartitioning = true,
                    DefaultMessageTimeToLive = TimeSpan.FromDays(5)
                };
                await _administrationClient.CreateTopicAsync(createQueueOptions, cancellationToken);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _receiver = _serviceBusClient.CreateReceiver(TopicName, SubscriptionName);

            _logger.LogInformation("Worker {Worker} running at: {Time}", nameof(AccountUpdatedSubscriptionWorker),
                DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                var receivedMessages = await _receiver.ReceiveMessagesAsync(100, cancellationToken: stoppingToken);
                if (receivedMessages is not {Count: > 0})
                    continue;

                foreach (var receivedMessage in receivedMessages)
                {
                    using var stream = new MemoryStream(receivedMessage.Body.ToArray());
                    var messageValue = await JsonSerializer.DeserializeAsync(
                            stream,
                            typeof(AccountCreated), cancellationToken: stoppingToken)
                        .ConfigureAwait(false);

                    _receiver.CompleteMessageAsync(receivedMessage, stoppingToken).FireAndForget();
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _serviceBusClient.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }

    internal static class TaskExtensions
    {
        public static void FireAndForget(this Task task, Action<Exception>? errorHandler = null)
        {
            task.ContinueWith(t =>
            {
                if (t is {IsFaulted: true, Exception: { }} && errorHandler != null)
                {
                    errorHandler(t.Exception);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public static void FireAndForget<T>(this Task<T> task, Action<Exception>? errorHandler = null)
        {
            task.ContinueWith(t =>
            {
                if (t is {IsFaulted: true, Exception: { }} && errorHandler != null)
                {
                    errorHandler(t.Exception);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }

    public class AccountCreated
    {
        public string AccountNumber { get; }
        public DateTime CreatedAt { get; }

        public AccountCreated(string accountNumber)
        {
            CreatedAt = DateTime.Now;
            AccountNumber = accountNumber;
        }
    }
}