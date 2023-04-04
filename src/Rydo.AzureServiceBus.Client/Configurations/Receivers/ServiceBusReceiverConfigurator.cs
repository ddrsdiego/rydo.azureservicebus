namespace Rydo.AzureServiceBus.Client.Configurations.Receivers
{
    using System;
    using Client.Producers;
    using Consumers.Subscribers;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public sealed class ServiceBusReceiverConfigurator
    {
        private readonly IServiceCollection _collection;

        internal ServiceBusReceiverConfigurator(IServiceCollection collection)
        {
            _collection = collection;
            ListenerContainer = new ReceiverListenerContainer();
            ReceiverContextContainer = new ReceiverContextContainer(collection);
        }

        internal IReceiverContextContainer ReceiverContextContainer { get; }

        internal IReceiverListenerContainer ListenerContainer { get; }

        /// <summary>
        /// Configures a listener to the context having as a parameter the topic name assigned to the consumer handler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Configure<T>() where T : class, IConsumerHandler => Configure<T>(c => c.Subscriber.Add());

        /// <summary>
        /// Configures a listener to the context having as a parameter the topic name assigned to the consumer handler.
        /// </summary>
        /// <param name="container"></param>
        /// <typeparam name="T"></typeparam>
        public void Configure<T>(Action<IReceiverContextContainer> container) where T : class, IConsumerHandler
        {
            if (!typeof(T).TryExtractTopicNameFromConsumer(out var topicName))
                throw new InvalidOperationException("");

            ReceiverContextContainer.Subscriber.WithConsumerHandler<T>();
            container(ReceiverContextContainer);
            _collection.TryAddScoped<T>();
        }
    }
}