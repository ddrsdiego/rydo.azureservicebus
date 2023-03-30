namespace Rydo.AzureServiceBus.Client.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Consumers;
    using Microsoft.Extensions.Hosting;

    internal sealed class AzureServiceBusIntegrationHostedService : BackgroundService
    {
        private const int MillisecondsDelayToStartConsumer = 5_000;
        private readonly ISubscriberContainer _subscriberContainer;
        private readonly IConsumerContextContainer _consumerContextContainer;

        public AzureServiceBusIntegrationHostedService(IConsumerContextContainer consumerContextContainer,
            ISubscriberContainer subscriberContainer)
        {
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
                    if (!_consumerContextContainer.TryGetConsumerContext(topicName, out var consumerContext))
                        continue;

                    if (!await subscriber.IsRunning)
                        // DEFINE STRATEGY TO STOP THE LISTENER
                        continue;
                    
                    subscriber.IsRunning = Task.Run(async () => await subscriber.StartAsync(stoppingToken),
                        stoppingToken);
                }
            }
        }
    }
}