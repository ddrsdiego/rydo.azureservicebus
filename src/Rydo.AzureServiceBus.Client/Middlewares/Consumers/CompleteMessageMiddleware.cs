namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Client.Consumers.Subscribers;
    using Handlers;
    using Logging;

    internal sealed class CompleteMessageMiddleware : MessageMiddleware
    {
        public CompleteMessageMiddleware()
            : base(nameof(CompleteMessageMiddleware))
        {
        }

        protected override string ConsumerMessagesStep => LogTypeConstants.CompleteMessageStep;

        protected override async Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            var completeMessageTasks = new Dictionary<MessageContext, Task>(context.Length);

            foreach (var messageContext in context.MessagesContext)
            {
                var completeMessageTask =
                    context.Receiver.CompleteMessageAsync(messageContext.ServiceBusMessageContext, context.CancellationToken);

                completeMessageTasks.Add(messageContext, completeMessageTask);
            }

            foreach (var task in completeMessageTasks)
            {
                try
                {
                    if (task.Value.IsCompletedSuccessfully) continue;
                    await SlowCompleteMessage(task.Value);
                }
                catch (Exception e)
                {
                }
            }

            static async Task SlowCompleteMessage(Task task) => await task;
        }
    }
}