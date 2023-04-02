namespace Rydo.AzureServiceBus.Client.Configurations.Receivers
{
    using System;
    using Client.Producers;
    using Consumers.Subscribers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public sealed class ServiceBusReceiverConfigurator
    {
        private readonly IServiceCollection _services;

        internal ServiceBusReceiverConfigurator(IServiceCollection services)
        {
            _services = services;
            ReceiverListenerContainer = new ReceiverListenerContainer();
            ReceiverContextContainer = new ReceiverContextContainer(services);
        }

        internal IReceiverContextContainer ReceiverContextContainer { get; }

        internal IReceiverListenerContainer ReceiverListenerContainer { get; }

        /// <summary>
        /// Configures a listener to the context having as a parameter the topic name assigned to the consumer handler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Configure<T>() => Configure<T>(c => c.Subscriber.Add());

        /// <summary>
        /// Configures a listener to the context having as a parameter the topic name assigned to the consumer handler.
        /// </summary>
        /// <param name="container"></param>
        /// <typeparam name="T"></typeparam>
        public void Configure<T>(Action<IReceiverContextContainer> container)
        {
            if (!typeof(T).TryExtractTopicNameFromConsumer(out var topicName))
                throw new InvalidOperationException("");

            ReceiverContextContainer.Subscriber.WithConsumerHandler<T>();
            container(ReceiverContextContainer);

            _services.TryAddScoped(typeof(T));
        }
    }
}