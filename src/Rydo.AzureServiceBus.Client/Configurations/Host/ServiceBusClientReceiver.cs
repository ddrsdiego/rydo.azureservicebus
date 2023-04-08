namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Consumers.Subscribers;

    public interface IServiceBusClientReceiver : IAsyncDisposable
    {
        void TryCreateReceiver(string queueName, ServiceBusReceiverOptions options);
        
        Task CompleteMessageAsync(ServiceBusMessageContext serviceBusMessageContext,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<ServiceBusMessageContext> StartConsumerAsync(CancellationToken cancellationToken);
    }

    internal sealed class ServiceBusClientReceiver : IServiceBusClientReceiver
    {
        private readonly object _lockObject;
        private readonly IServiceBusHostSettings _hostSettings;
        
        private ServiceBusReceiver _serviceBusReceiver;
        
        internal ServiceBusClientReceiver(IServiceBusHostSettings hostSettings)
        {
            _hostSettings = hostSettings ?? throw new ArgumentNullException(nameof(hostSettings));
            _lockObject = new object();
        }

        public Task CompleteMessageAsync(ServiceBusMessageContext serviceBusMessageContext,
            CancellationToken cancellationToken = default)
        {
            return _serviceBusReceiver.CompleteMessageAsync(serviceBusMessageContext.Message, cancellationToken);
        }

        public void TryCreateReceiver(string queueName, ServiceBusReceiverOptions options)
        {
            EnsureReceiver(queueName, options);
        }

        public async IAsyncEnumerable<ServiceBusMessageContext> StartConsumerAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var serviceBusReceivedMessage in _serviceBusReceiver.ReceiveMessagesAsync(cancellationToken))
            {
                if (serviceBusReceivedMessage == null) continue;
                yield return new ServiceBusMessageContext(serviceBusReceivedMessage);
            }
        }

        private void EnsureReceiver(string queueName, ServiceBusReceiverOptions options)
        {
            if (_serviceBusReceiver != null)
                return;

            lock (_lockObject)
            {
                _serviceBusReceiver = _hostSettings.ServiceBusClient.CreateReceiver(queueName, options);
            }
        }

        public async ValueTask DisposeAsync()
        {
            lock (_lockObject)
            {
                if (_serviceBusReceiver == null || _serviceBusReceiver.IsClosed)
                    return;
            }

            await _serviceBusReceiver.DisposeAsync();
        }
    }
}