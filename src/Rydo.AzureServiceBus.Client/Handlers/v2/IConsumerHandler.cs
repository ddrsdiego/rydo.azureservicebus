namespace Rydo.AzureServiceBus.Client.Handlers.v2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Utils;

    public interface IConsumerHandler
    {
    }

    public interface IConsumerHandler<TMessage> :
        IConsumerHandler
    {
        Task Handle(IMessageConsumerContext<TMessage> context, CancellationToken cancellationToken);
    }
    
    public abstract class ConsumerHandler<TMessage> : IConsumerHandler<TMessage>
    {
        protected ConsumerHandler()
        {
            HandlerId = GeneratorOperationId.Generate();
        }

        // some fields that require cleanup
        private bool _disposed = false; // to detect redundant calls

        public string HandlerId { get; }


        public abstract Task Handle(IMessageConsumerContext<TMessage> context, CancellationToken cancellationToken);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // dispose-only, i.e. non-finalizable logic
            }

            // shared cleanup logic
            _disposed = true;
        }

        ~ConsumerHandler()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}