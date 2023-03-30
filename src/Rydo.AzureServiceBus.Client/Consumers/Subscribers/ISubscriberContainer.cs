namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Collections.Immutable;

    public interface ISubscriberContainer
    {
        IServiceProvider Provider { get; }

        void SetServiceProvider(IServiceProvider provider);

        public void AddSubscriber(string topicName, ISubscriber subscriber);

        ImmutableDictionary<string, ISubscriber> Listeners { get; }
    }
}