namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers.Factories
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;
    using Client.Consumers.MessageRecordModel;
    using Client.Consumers.Subscribers;
    using Handlers;

    internal abstract class ConsumerContextFactory
    {
        private static readonly ConcurrentDictionary<Type, Lazy<ConsumerContextFactory>> Executors =
            new ConcurrentDictionary<Type, Lazy<ConsumerContextFactory>>();

        protected ConsumerContextFactory(Type messageType) => MessageType = messageType;

        private Type MessageType { get; }

        public static ConsumerContextFactory GetConsumerContext(Type messageType)
        {
            var consumerContextFactory = Executors.GetOrAdd(messageType, _ =>
            {
                return new Lazy<ConsumerContextFactory>(() =>
                    (ConsumerContextFactory) Activator.CreateInstance(
                        typeof(InnerConsumerContext<>).MakeGenericType(messageType), messageType));
            });

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
                var items = new MessageRecord<TMessage>[consumerContext.MessagesContext.Length];

                for (var index = 0; index < consumerContext.MessagesContext.Length; index++)
                {
                    var messageContext = consumerContext.MessagesContext[index];
                    var messageRecord = ((MessageContext) messageContext).Record;
                    if (messageRecord.IsInvalid)
                        continue;

                    items[index] = messageRecord.GetMessageRecordTyped<TMessage>() as MessageRecord<TMessage>;
                }

                return new ConsumeContextScope<TMessage>(new ConsumerContext<TMessage>(consumerContext, items));
            }
        }
    }
}