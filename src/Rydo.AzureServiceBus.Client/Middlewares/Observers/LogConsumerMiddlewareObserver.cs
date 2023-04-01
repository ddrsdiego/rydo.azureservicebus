namespace Rydo.AzureServiceBus.Client.Middlewares.Observers
{
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Handlers;
    using Logging;
    using Microsoft.Extensions.Logging;

    internal class LogConsumerMiddlewareObserver : IConsumerMiddlewareObserver
    {
        private readonly ILogger<LogConsumerMiddlewareObserver> _logger;

        public LogConsumerMiddlewareObserver(ILogger<LogConsumerMiddlewareObserver> logger)
        {
            _logger = logger;
        }

        public Task PreConsumer(string middlewareType, string step, MessageConsumerContext context)
        {
            var stepFormat = $"{step}-INIT";
            
            context.StarWatch();

            var messageAudit = ConsumeMetadataFactory.CreateAuditMetadata(context, middlewareType, stepFormat);
            _logger.LogInformation(
                $"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MsgConsumerContextAuditMedata}",
                step,
                messageAudit);

            return Task.CompletedTask;
        }

        public Task PostConsumer(string middlewareType, string step, MessageConsumerContext context)
        {
            var stepFormat = $"{step}-END";
            
            context.StopWatch();
            
            var messageAudit = ConsumeMetadataFactory.CreateAuditMetadata(context, middlewareType, stepFormat);
            _logger.LogInformation(
                $"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MsgConsumerContextAuditMedata}",
                step,
                messageAudit);

            return Task.CompletedTask;
        }
    }

    internal static class ConsumeMetadataFactory
    {
        public static MessageConsumerContextAuditMedata CreateAuditMetadata(MessageConsumerContext context,
            string middlewareType, string step)
        {
            return new MessageConsumerContextAuditMedata
            {
                Step = step,
                ContextId = context.ContextId,
                ContextLength = context.Length,
                MiddlewareType = middlewareType,
                ElapsedTime = context.ElapsedTimeConsumer,
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
    }
}