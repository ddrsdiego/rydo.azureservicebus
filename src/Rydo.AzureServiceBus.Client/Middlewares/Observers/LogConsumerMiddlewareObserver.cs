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
            _logger.LogInformation($"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MiddlewareType} - {ServiceBusLogFields.MessageConsumerContextLength}.",
                step,
                middlewareType,
                context.Count);

            return Task.CompletedTask;
        }

        public Task PostConsumer(string middlewareType, string step, MessageConsumerContext context)
        {
            var messageAudit = ConsumeMetadataFactory.CreateAuditMetadata(context, middlewareType, step);

            _logger.LogInformation($"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MsgConsumerContextAuditMedata}",
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
                MiddlewareType = middlewareType,
                ElapsedTimeConsumer = context.ElapsedTimeConsumer,
            };
        }
    }

    public class MessageConsumerContextAuditMedata
    {
        public string MiddlewareType { get; set; }
        public string Step { get; set; }
        public long ElapsedTimeConsumer { get; set; }
    }
}