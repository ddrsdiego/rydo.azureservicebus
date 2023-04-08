namespace Rydo.AzureServiceBus.Client.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Consumers.Subscribers;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    internal sealed class AzureServiceBusIntegrationHostedService : BackgroundService
    {
        private const int MillisecondsDelayToStartConsumer = 5_000;

        private readonly CancellationTokenSource _source;
        private readonly IReceiverListenerContainer _receiverListenerContainer;
        private readonly ILogger<AzureServiceBusIntegrationHostedService> _logger;
        private readonly ISubscriberContextContainer _subscriberContextContainer;

        public AzureServiceBusIntegrationHostedService(ILogger<AzureServiceBusIntegrationHostedService> logger,
            ISubscriberContextContainer subscriberContextContainer,
            IReceiverListenerContainer receiverListenerContainer)
        {
            _source = new CancellationTokenSource();
            _logger = logger;
            _subscriberContextContainer = subscriberContextContainer;
            _receiverListenerContainer = receiverListenerContainer;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var (topicName, receiverListener) in _receiverListenerContainer.Listeners)
            {
                if (!_subscriberContextContainer.TryGetConsumerContext(topicName, out var consumerContext))
                    continue;

                await receiverListener.BusClient.Admin.CreateEntitiesIfNotExistAsync(consumerContext, _source.Token);
            }

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(MillisecondsDelayToStartConsumer, stoppingToken);

            while (!_source.Token.IsCancellationRequested)
            {
                foreach (var (topicName, receiverListener) in _receiverListenerContainer.Listeners)
                {
                    if (!receiverListener.IsRunning.IsCompleted)
                        continue;

                    if (!_subscriberContextContainer.TryGetConsumerContext(topicName, out var subscriberContext))
                        continue;

                    if (!await receiverListener.IsRunning)
                    {
                        // DEFINE STRATEGY TO STOP THE LISTENER
                        continue;
                    }

                    receiverListener.IsRunning = Task.Run(async () => await receiverListener.StartAsync(_source.Token),
                        stoppingToken);
                }

                await Task.Delay(100, _source.Token);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _source.Cancel();

            foreach (var (topic, receiverListener) in _receiverListenerContainer.Listeners)
            {
                await receiverListener.StopAsync(cancellationToken);
            }

            await base.StopAsync(cancellationToken);
        }
    }
}