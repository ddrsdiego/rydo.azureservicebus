namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.Logging;

    internal sealed class CompleteMessageMiddleware : MessageMiddleware
    {
        public CompleteMessageMiddleware(ILoggerFactory logger)
            : base(logger.CreateLogger<CompleteMessageMiddleware>())
        {
        }

        public override async Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();

            var completeMessageTasks = new List<Task>(context.Count);
            foreach (var messageContext in context.MessageContexts)
            {
                completeMessageTasks.Add(
                    context.Receiver.CompleteMessageAsync(messageContext.ReceivedMessage, cancellationToken));
            }

            var tasks = completeMessageTasks.ToArray();
            for (var index = 0; index < tasks.Length; index++)
            {
                var task = tasks[index];

                if (task.IsCompletedSuccessfully)
                    continue;

                Logger.LogWarning("[{LogType}]",
                    "SLOW_COMPLETE_MESSAGE");
                await SlowCompleteMessage(task);
            }

            sw.Stop();
            
            Logger.LogInformation("[{LogType}] - {TasksLength} messages completed in {ElapsedMilliseconds} ms.",
                "COMPLETED_MESSAGE",
                tasks.Length, 
                sw.ElapsedMilliseconds);

            static async Task SlowCompleteMessage(Task task) => await task;

            await next(context);
        }
    }
}