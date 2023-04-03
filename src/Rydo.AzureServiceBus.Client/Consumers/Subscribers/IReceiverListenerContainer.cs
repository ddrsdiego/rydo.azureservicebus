namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Collections.Immutable;

    public interface IReceiverListenerContainer
    {
        IServiceProvider Provider { get; }

        void SetServiceProvider(IServiceProvider provider);

        public void TryAddListener(string topicName, IReceiverListener receiverListener);

        ImmutableDictionary<string, IReceiverListener> Listeners { get; }
    }
}