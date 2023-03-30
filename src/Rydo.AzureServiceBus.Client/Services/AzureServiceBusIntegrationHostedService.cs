namespace Rydo.AzureServiceBus.Client.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Consumers;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Subscribers;

    internal sealed class AzureServiceBusIntegrationHostedService : BackgroundService
    {
        private const int MillisecondsDelayToStartConsumer = 5_000;
        
        private readonly CancellationTokenSource _source;
        private readonly ISubscriberContainer _subscriberContainer;
        private readonly ILogger<AzureServiceBusIntegrationHostedService> _logger;
        private readonly IConsumerContextContainer _consumerContextContainer;

        public AzureServiceBusIntegrationHostedService(ILogger<AzureServiceBusIntegrationHostedService> logger,
            IConsumerContextContainer consumerContextContainer,
            ISubscriberContainer subscriberContainer)
        {
            _source = new CancellationTokenSource();
            _logger = logger;
            _consumerContextContainer = consumerContextContainer;
            _subscriberContainer = subscriberContainer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(MillisecondsDelayToStartConsumer, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var (topicName, subscriber) in _subscriberContainer.Listeners)
                {
                    if (!subscriber.IsRunning.IsCompleted)
                        continue;
                    
                    if (!_consumerContextContainer.TryGetConsumerContext(topicName, out var consumerContext))
                        continue;

                    if (!await subscriber.IsRunning)
                    {
                        // DEFINE STRATEGY TO STOP THE LISTENER
                        continue;
                    }
                    
                    subscriber.IsRunning = Task.Run(async () => await subscriber.StartAsync(stoppingToken),
                        stoppingToken);
                }
                
                await Task.Delay(1_000, _source.Token);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}