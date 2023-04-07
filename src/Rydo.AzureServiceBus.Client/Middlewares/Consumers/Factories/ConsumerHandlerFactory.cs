namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers.Factories
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Handlers;

    public abstract class ConsumerHandlerFactory
    {
        private static readonly ConcurrentDictionary<Type, ConsumerHandlerFactory> Executors =
            new ConcurrentDictionary<Type, ConsumerHandlerFactory>();

        public static ConsumerHandlerFactory GetExecutor(Type messageType)
        {
            return Executors.GetOrAdd(
                messageType,
                _ => (ConsumerHandlerFactory) Activator.CreateInstance(
                    typeof(InnerConsumerHandlerFactory<>).MakeGenericType(messageType)));
        }

        public abstract Task Execute(object handler, IConsumerContext consumerContext);

        private class InnerConsumerHandlerFactory<TMessage> : ConsumerHandlerFactory
        {
            public override Task Execute(object handler, IConsumerContext consumerContext)
            {
                var typedConsumerHandler = (IConsumerHandler<TMessage>) handler;
                var typedConsumerContext = (IConsumerContext<TMessage>) consumerContext;

                return typedConsumerHandler.Consume(typedConsumerContext);
            }
        }
    }
}