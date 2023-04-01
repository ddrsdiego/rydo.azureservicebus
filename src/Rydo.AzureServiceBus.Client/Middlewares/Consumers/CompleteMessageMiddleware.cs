namespace Rydo.AzureServiceBus.Client.Middlewares.Consumers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.Logging;

    internal sealed class CompleteMessageMiddleware : MessageMiddleware
    {
        public CompleteMessageMiddleware(ILoggerFactory logger)
            : base(logger.CreateLogger<CompleteMessageMiddleware>())
        {
        }

        public override async Task InvokeAsync(MessageConsumerContext context, MiddlewareDelegate next)
        {
            var sw = Stopwatch.StartNew();

            context.StarWatch();
            
            if (ConsumerMiddlewareObservable.Count > 0)
                await ConsumerMiddlewareObservable.PreConsumer(nameof(CompleteMessageMiddleware),"START-COMPLETE-MESSAGE", context);
            
            var completeMessageTasks = new List<Task>(context.Count);
            foreach (var messageContext in context.MessageContexts)
            {
                completeMessageTasks.Add(
                    context.Receiver.CompleteMessageAsync(messageContext.ReceivedMessage, context.CancellationToken));
            }

            var tasks = completeMessageTasks.ToArray();
            for (var index = 0; index < tasks.Length; index++)
            {
                var task = tasks[index];

                if (task.IsCompletedSuccessfully)
                    continue;

                await SlowCompleteMessage(task);
            }

            context.StopWatch();
            if (ConsumerMiddlewareObservable.Count > 0)
                await ConsumerMiddlewareObservable.PostConsumer(nameof(CompleteMessageMiddleware),"FINISH-COMPLETE-MESSAGE", context);
            
            static async Task SlowCompleteMessage(Task task) => await task;

            await next(context);
        }
    }
}