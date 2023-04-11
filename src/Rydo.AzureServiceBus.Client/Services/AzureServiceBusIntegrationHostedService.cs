namespace Rydo.AzureServiceBus.Client.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Consumers.Subscribers;
    using Microsoft.Extensions.Hosting;

    internal sealed class AzureServiceBusIntegrationHostedService : BackgroundService
    {
        private const int MillisecondsDelayToStartConsumer = 5_000;

        private readonly CancellationTokenSource _stopCancellationTokenSource;
        private readonly IReceiverListenerContainer _receiverListenerContainer;
        private readonly ISubscriberContextContainer _subscriberContextContainer;

        public AzureServiceBusIntegrationHostedService(ISubscriberContextContainer subscriberContextContainer,
            IReceiverListenerContainer receiverListenerContainer)
        {
            _stopCancellationTokenSource = new CancellationTokenSource();
            _subscriberContextContainer = subscriberContextContainer;
            _receiverListenerContainer = receiverListenerContainer;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var (topicName, receiverListener) in _receiverListenerContainer.Listeners)
            {
                if (!_subscriberContextContainer.TryGetConsumerContext(topicName, out var consumerContext))
                    continue;

                await receiverListener.CreateEntitiesIfNotExistAsync(consumerContext,
                    _stopCancellationTokenSource.Token);
            }

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(MillisecondsDelayToStartConsumer, stoppingToken);

            while (!_stopCancellationTokenSource.Token.IsCancellationRequested)
            {
                foreach (var (_, receiverListener) in _receiverListenerContainer.Listeners)
                {
                    try
                    {
                        if (!receiverListener.IsRunning.IsCompleted)
                            continue;

                        if (!await receiverListener.IsRunning)
                        {
                            // DEFINE STRATEGY TO STOP THE LISTENER
                            continue;
                        }

                        receiverListener.IsRunning = Task.Run(async () =>
                                await receiverListener.StartAsync(_stopCancellationTokenSource),
                            _stopCancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                await Task.Delay(100, _stopCancellationTokenSource.Token);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_stopCancellationTokenSource.Token.CanBeCanceled)
                _stopCancellationTokenSource.Cancel();

            foreach (var (_, receiverListener) in _receiverListenerContainer.Listeners)
            {
                try
                {
                    await receiverListener.StopAsync(_stopCancellationTokenSource);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            await base.StopAsync(cancellationToken);
        }
    }
}