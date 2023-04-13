namespace Rydo.AzureServiceBus.Client.Middlewares.Observers
{
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Handlers;
    using Logging;
    using Microsoft.Extensions.Logging;

    internal sealed class LogConsumerMiddlewareObserver : IConsumerMiddlewareObserver
    {
        private readonly ILogger<LogConsumerMiddlewareObserver> _logger;

        public LogConsumerMiddlewareObserver(ILoggerFactory logger) =>
            _logger = logger.CreateLogger<LogConsumerMiddlewareObserver>();

        public Task PreConsumerAsync(string middlewareType, string step, MessageConsumerContext context)
        {
            var stepFormat = $"{step}-init";

            context.StarMiddlewareWatch();

            var messageAudit = ConsumeMetadataFactory.CreateAuditMetadata(context, middlewareType, stepFormat);
            _logger.LogDebug(
                $"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MessageConsumerContextAuditLog}",
                step,
                messageAudit);

            return Task.CompletedTask;
        }

        public Task PostConsumerAsync(string middlewareType, string step, MessageConsumerContext context)
        {
            context.StopMiddlewareWatch();

            var stepFormat = $"{step}-end";

            var messageAudit = ConsumeMetadataFactory.CreateAuditMetadata(context, middlewareType, stepFormat);
            _logger.LogInformation(
                $"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MessageConsumerContextAuditLog}",
                step,
                messageAudit);

            return Task.CompletedTask;
        }

        public Task EndConsumerAsync(string middlewareType, string step, MessageConsumerContext context) =>
            Task.CompletedTask;
    }

    internal static class ConsumeMetadataFactory
    {
        public static MessageConsumerContextAuditMedata CreateAuditMetadata(MessageConsumerContext context,
            string middlewareType, string step)
        {
            return new MessageConsumerContextAuditMedata
            {
                Step = step.ToLowerInvariant(),
                Queue = context.Queue,
                Topic = context.Topic,
                Subscription = context.Subscription,
                ContextId = context.ContextId,
                ContextLength = context.Length,
                MiddlewareType = middlewareType,
                ElapsedTime = context.ElapsedTimeMiddleware,
            };
        }
    }

    internal sealed class MessageConsumerContextAuditMedata
    {
        public string ContextId { get; set; }
        public string MiddlewareType { get; set; }
        public long ContextLength { get; set; }
        public string Step { get; set; }
        public long ElapsedTime { get; set; }
        public string Queue { get; set; }
        public string Topic { get; set; }
        public string Subscription { get; set; }
    }
}