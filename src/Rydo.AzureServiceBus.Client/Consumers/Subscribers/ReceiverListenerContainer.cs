namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Middlewares;

    public sealed class ReceiverListenerContainer : IReceiverListenerContainer
    {
        public ReceiverListenerContainer()
        {
            Listeners = ImmutableDictionary<string, IReceiverListener>.Empty;
        }

        public ImmutableDictionary<string, IReceiverListener> Listeners { get; private set; }

        public IServiceProvider Provider { get; private set; }

        public void SetServiceProvider(IServiceProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));

            foreach (var subscriber in Listeners.Values)
            {
                subscriber
                    .MiddleExecutor(BuildMiddlewareExecutor(provider))
                    .ServiceProvider(provider);
            }
        }

        public void TryAddListener(string topicName, IReceiverListener receiverListener)
        {
            if (topicName == null || string.IsNullOrEmpty(topicName))
                throw new ArgumentNullException(nameof(topicName));

            if (Listeners.TryGetValue(topicName, out _))
                throw new InvalidOperationException(nameof(topicName));

            Listeners = Listeners.Add(topicName, receiverListener);
        }

        private static IMiddlewareExecutor BuildMiddlewareExecutor(IServiceProvider provider) =>
            MiddlewareExecutor.Builder()
                .WithLogger(provider.GetRequiredService<ILogger<MiddlewareExecutor>>())
                .WithMiddlewareConfigContainer(provider.GetRequiredService<IMiddlewareConfigurationContainer>())
                .Build();
    }
}