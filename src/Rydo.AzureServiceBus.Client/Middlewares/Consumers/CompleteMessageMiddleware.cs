namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.Logging;

    internal sealed class CompleteMessageMiddleware : MessageMiddleware
    {
        private const string CompleteMessageStep = "COMPLETE-CONSUMER-MESSAGE";

        public CompleteMessageMiddleware(ILoggerFactory logger)
            : base(logger.CreateLogger<CompleteMessageMiddleware>())
        {
        }

        public override async Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            if (ConsumerMiddlewareObservable.Count > 0)
                await ConsumerMiddlewareObservable.PreConsumer(nameof(CompleteMessageMiddleware), CompleteMessageStep,
                    context);

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

            if (ConsumerMiddlewareObservable.Count > 0)
                await ConsumerMiddlewareObservable.PostConsumer(nameof(CompleteMessageMiddleware), CompleteMessageStep,
                    context);

            static async Task SlowCompleteMessage(Task task) => await task;

            await next(context);
        }
    }
}