namespace Rydo.AzureServiceBus.Client.Handlers
{
    using System;
    using System.Threading.Tasks;

    public interface IConsumerHandler
    {
        string HandlerId { get; }
        
        Task HandleAsync(MessageConsumerContext context);
    }
    
    public interface IConsumerHandler<TMessage> : IConsumerHandler
    {
    }
    
    public abstract class ConsumerHandler<TMessage> : IConsumerHandler<TMessage>
    {
        protected ConsumerHandler()
        {
            HandlerId = Guid.NewGuid().ToString().Split('-')[0];
        }

        // some fields that require cleanup
        private bool _disposed = false; // to detect redundant calls

        public string HandlerId { get; }

        public abstract Task HandleAsync(MessageConsumerContext context);

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