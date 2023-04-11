namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers.Factories
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Handlers;

    internal abstract class ConsumerHandlerFactory
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

        private class InnerConsumerHandlerFactory<TMessage> : ConsumerHandlerFactory where TMessage : class
        {
            public override Task Execute(object handler, IConsumerContext consumerContext)
            {
                var typedConsumerHandler =  handler as IConsumerHandler<TMessage>;
                var typedConsumerContext =  consumerContext as IConsumerContext<TMessage>;

                return typedConsumerHandler.Consume(typedConsumerContext);
            }
        }
    }
}