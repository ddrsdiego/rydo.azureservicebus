﻿namespace Rydo.AzureServiceBus.Client.Middlewares.Observers
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
            var stepFormat = $"{step}-INIT";

            context.StarMiddlewareWatch();

            var messageAudit = ConsumeMetadataFactory.CreateAuditMetadata(context, middlewareType, stepFormat);
            _logger.LogDebug(
                $"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MsgConsumerContextAuditMedata}",
                step,
                messageAudit);

            return Task.CompletedTask;
        }

        public Task PostConsumerAsync(string middlewareType, string step, MessageConsumerContext context)
        {
            context.StopMiddlewareWatch();

            var stepFormat = $"{step}-END";
            
            var messageAudit = ConsumeMetadataFactory.CreateAuditMetadata(context, middlewareType, stepFormat);
            _logger.LogDebug(
                $"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MsgConsumerContextAuditMedata}",
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
                Step = step,
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
    }
}