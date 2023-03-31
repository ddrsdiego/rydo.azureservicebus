namespace Rydo.AzureServiceBus.Consumer.Workers
{
    using System.Text.Json;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;
    using Client.Consumers.Subscribers;

    internal sealed class AccountUpdatedSubscriptionWorker : BackgroundService
    {
        private const string SubscriptionName = "rydo.azureservicebus.consumer";
        private const string TopicName = "rydo-azureservicebus-account-updated";

        private ServiceBusReceiver? _receiver;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusAdministrationClient _administrationClient;
        private readonly ISubscriberContextContainer _subscriberContextContainer;
        private readonly ILogger<AccountUpdatedSubscriptionWorker> _logger;

        public AccountUpdatedSubscriptionWorker(ILogger<AccountUpdatedSubscriptionWorker> logger,
            ServiceBusClient serviceBusClient,
            ServiceBusAdministrationClient administrationClient,
            ISubscriberContextContainer subscriberContextContainer)
        {
            _logger = logger;
            _serviceBusClient = serviceBusClient;
            _administrationClient = administrationClient;
            _subscriberContextContainer = subscriberContextContainer;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitiateEntities(cancellationToken);
            await base.StartAsync(cancellationToken);
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
    }
}