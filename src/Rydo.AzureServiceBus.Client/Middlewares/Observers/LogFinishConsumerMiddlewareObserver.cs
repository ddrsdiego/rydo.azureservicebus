namespace Rydo.AzureServiceBus.Client.Middlewares.Observers
{
    using System.Threading.Tasks;
    using Handlers;
    using Logging;
    using Microsoft.Extensions.Logging;

    internal sealed class LogFinishConsumerMiddlewareObserver : IFinishConsumerMiddlewareObserver
    {
        private readonly ILogger _logger;

        public LogFinishConsumerMiddlewareObserver(ILoggerFactory logger) =>
            _logger = logger.CreateLogger<LogFinishConsumerMiddlewareObserver>();

        public Task EndConsumerAsync(MessageConsumerContext context)
        {
            context.StopMsgContextWatch();

            var messageAudit = new MessageConsumerContextAuditLog(context);
            
            _logger.LogInformation(
                $"[{ServiceBusLogFields.LogType}] - {ServiceBusLogFields.MessageConsumerContextAuditLog}",
                LogTypeConstants.FinishConsumerContext,
                messageAudit);
            
            return Task.CompletedTask;
        }
    }

    internal sealed class MessageConsumerContextAuditLog
    {
        private readonly MessageConsumerContext _context;

        public MessageConsumerContextAuditLog(MessageConsumerContext context) => _context = context;

        public string ContextId => _context.ContextId;
        public int ContextLength => _context.Length;
        public string Topic => _context.Topic;
        public string Subscription => _context.Subscription;
        public string Queue => _context.Queue;
        public string ContractMessage => _context.ContractType.ToString();
        public string HandlerContractMessage => _context.HandlerType.ToString();
        public long ElapsedTimeMessageContext => _context.ElapsedTimeMessageContext;
    }
}