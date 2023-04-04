namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Client.Consumers.Subscribers;
    using Handlers;
    using Handlers.v2;
    using Microsoft.Extensions.DependencyInjection;
    using IConsumerHandler = Handlers.IConsumerHandler;

    internal sealed class CustomConsumerMiddleware : MessageMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private const string CustomHandlerConsumerStep = "CUSTOM-HANDLER-CONSUMER-MESSAGES";

        public CustomConsumerMiddleware(IServiceProvider serviceProvider)
            : base(nameof(CustomConsumerMiddleware))
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override string ConsumerMessagesStep => CustomHandlerConsumerStep;

        protected override async Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            using var scope = _serviceProvider.CreateScope();
            if (scope.ServiceProvider.GetService(context.HandlerType ?? throw new InvalidOperationException()) is
                IConsumerHandler messageHandler)
            {
                try
                {
                    foreach (var messageContext in context.MessagesContext)
                    {
                        var messageRecord = (MessageRecord) Activator.CreateInstance(
                            typeof(MessageRecord<>).MakeGenericType(context.ContractType));

                        var messageExecutor = MessageAdapter.Adapt(context.ContractType);
                        messageExecutor.Execute(messageRecord, messageContext.Record.MessageValue);
                        
                        
                    }

                    await messageHandler.Handle(context, context.CancellationToken);
                }
                catch (Exception e)
                {
                }
            }
        }
    }

    internal abstract class MessageAdapter
    {
        private static readonly ConcurrentDictionary<Type, MessageAdapter> Executors =
            new ConcurrentDictionary<Type, MessageAdapter>();

        public static MessageAdapter Adapt(Type messageType)
        {
            return Executors.GetOrAdd(
                messageType,
                _ => (MessageAdapter) Activator.CreateInstance(
                    typeof(InnerMessageAdapter<>).MakeGenericType(messageType)));
        }

        public abstract void Execute(object messageRecord, object message);

        private class InnerMessageAdapter<T> : MessageAdapter
        {
            public override void Execute(object messageRecord, object message)
            {
                var messageRecordGeneric = messageRecord as MessageRecord<T>;
                messageRecordGeneric.SetMessage((T) message);
            }
        }
    }
}