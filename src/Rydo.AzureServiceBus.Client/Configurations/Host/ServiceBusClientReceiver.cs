namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Consumers.Subscribers;
    using Microsoft.Extensions.Logging;
    using Utils;

    public interface IServiceBusClientReceiver : IAsyncDisposable
    {
        void TryCreateReceiver(SubscriberContext subscriberContext);

        Task CompleteMessageAsync(IServiceBusMessageContext serviceBusMessageContext,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<IServiceBusMessageContext> StartConsumerAsync(CancellationToken cancellationToken);
    }

    internal sealed class ServiceBusClientReceiver : IServiceBusClientReceiver
    {
        private readonly object _lockObject;
        private readonly ILogger<ServiceBusClientReceiver> _logger;
        private readonly IServiceBusHostSettings _hostSettings;
        private readonly SemaphoreSlim _semaphoreSlim;
        
        private ServiceBusReceiver _serviceBusReceiver;
        private SubscriberContext _subscriberContext;

        internal ServiceBusClientReceiver(ILogger<ServiceBusClientReceiver> logger,
            IServiceBusHostSettings hostSettings)
        {
            _logger = logger;
            _hostSettings = hostSettings ?? throw new ArgumentNullException(nameof(hostSettings));
            
            _lockObject = new object();
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public Task CompleteMessageAsync(IServiceBusMessageContext serviceBusMessageContext,
            CancellationToken cancellationToken = default)
        {
            var messageContext = (ServiceBusMessageContext) serviceBusMessageContext;
            return _serviceBusReceiver.CompleteMessageAsync(messageContext.Message, cancellationToken);
        }

        public void TryCreateReceiver(SubscriberContext subscriberContext) => EnsureReceiver(subscriberContext);

        public async IAsyncEnumerable<IServiceBusMessageContext> StartConsumerAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var receiveMessages =
                await _serviceBusReceiver.ReceiveMessagesAsync(_subscriberContext.Specification.PrefetchCount, cancellationToken: cancellationToken);

            foreach (var receivedMessage in receiveMessages)
            {
                if (receivedMessage == null) continue;
                yield return new ServiceBusMessageContext(receivedMessage);
            }
        }

        private void EnsureReceiver(SubscriberContext subscriberContext)
        {
            lock (_lockObject)
            {
                _subscriberContext ??= subscriberContext;

                if (_serviceBusReceiver != null)
                    return;

                var options = new ServiceBusReceiverOptions
                {
                    PrefetchCount = _subscriberContext.Specification.PrefetchCount,
                    Identifier = _subscriberContext.Specification.SubscriptionName,
                    ReceiveMode = _subscriberContext.Specification.ReceiveMode
                };

                _serviceBusReceiver =
                    _hostSettings.ServiceBusClient.CreateReceiver(_subscriberContext.Specification.QueueName, options);

                _logger.LogInformation("{Id} - {EntityPath}", GeneratorOperationId.Generate(),
                    _serviceBusReceiver.EntityPath);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                if (_serviceBusReceiver == null || _serviceBusReceiver.IsClosed)
                    return;
                
                await _serviceBusReceiver.DisposeAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}