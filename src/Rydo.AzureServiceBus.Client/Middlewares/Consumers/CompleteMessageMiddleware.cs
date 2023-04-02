namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Handlers;

    internal sealed class CompleteMessageMiddleware : MessageMiddleware
    {
        private const string CompleteMessageStep = "COMPLETE-CONSUMER-MESSAGE";

        public CompleteMessageMiddleware()
            : base(nameof(CompleteMessageMiddleware))
        {
        }

        protected override string ConsumerMessagesStep => CompleteMessageStep;

        protected override async Task ExecuteInvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            var completeMessageTasks = new List<Task>(context.Length);
            foreach (var messageContext in context.MessagesContext)
            {
                completeMessageTasks.Add(
                    context.Receiver.CompleteMessageAsync(messageContext.ReceivedMessage, context.CancellationToken));
            }

            var tasks = completeMessageTasks.ToArray();
            for (var index = 0; index < tasks.Length; index++)
            {
                var task = tasks[index];

                if (task.IsCompletedSuccessfully) continue;

                await SlowCompleteMessage(task);
            }

            static async Task SlowCompleteMessage(Task task) => await task;
        }
    }
}