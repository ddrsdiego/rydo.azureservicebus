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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(MillisecondsDelayToStartConsumer, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var (topicName, subscriber) in _receiverListenerContainer.Listeners)
                {
                    if (!subscriber.IsRunning.IsCompleted)
                        continue;
                    
                    if (!_subscriberContextContainer.TryGetConsumerContext(topicName, out var consumerContext))
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