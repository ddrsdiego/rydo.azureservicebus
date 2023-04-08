namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers.Factories
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Client.Consumers.Subscribers;
    using Handlers;

    public abstract class ConsumerContextFactory
    {
        private static readonly ConcurrentDictionary<Type, Lazy<ConsumerContextFactory>> Executors =
            new ConcurrentDictionary<Type, Lazy<ConsumerContextFactory>>();

        protected ConsumerContextFactory(Type messageType) => MessageType = messageType;

        private Type MessageType { get; }

        public static ConsumerContextFactory GetConsumerContext(Type messageType)
        {
            var consumerContextFactory = Executors.GetOrAdd(messageType, _ =>
                new Lazy<ConsumerContextFactory>(() =>
                    (ConsumerContextFactory) Activator.CreateInstance(
                        typeof(InnerConsumerContext<>).MakeGenericType(messageType), messageType)));

            return consumerContextFactory.Value;
        }

        public abstract IConsumerContext Execute(MessageConsumerContext consumerContext);

        private class InnerConsumerContext<TMessage> : ConsumerContextFactory
            where TMessage : class
        {
            public InnerConsumerContext(Type messageType)
                : base(messageType)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override IConsumerContext Execute(MessageConsumerContext consumerContext)
            {
                var messageRecordExecutor = MessageRecordFactory.GetConsumerContext(MessageType);
                var messageRecords = consumerContext.Messages.ToArray();

                var items = new MessageRecord<TMessage>[messageRecords.Length];
                for (var index = 0; index < messageRecords.Length; index++)
                {
                    var messageRecord = messageRecordExecutor.Execute(messageRecords[index]);

                    var item = (MessageRecord<TMessage>) messageRecord;
                    items[index] = item;
                }

                return new ConsumeContextScope<TMessage>(new ConsumerContext<TMessage>(consumerContext, items));
            }
        }
    }
}