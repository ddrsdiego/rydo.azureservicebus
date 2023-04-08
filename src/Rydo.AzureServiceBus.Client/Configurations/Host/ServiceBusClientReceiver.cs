namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;

    public interface IServiceBusClientReceiver : IAsyncDisposable
    {
        Task<IReadOnlyList<ServiceBusReceivedMessage>> ReceiveMessagesAsync(int maxMessages,
            CancellationToken cancellationToken = default);
        
        bool TryGet(string queueName, ServiceBusReceiverOptions options, out ServiceBusReceiver receiver);
    }

    internal sealed class ServiceBusClientReceiverContainer
    {
        private readonly object _lockObject;
        private ImmutableDictionary<string, IServiceBusClientReceiver> _receivers;

        public ServiceBusClientReceiverContainer()
        {
            _lockObject = new object();
            _receivers = ImmutableDictionary<string, IServiceBusClientReceiver>.Empty;
        }
    }
    
    
    internal sealed class ServiceBusClientReceiver : IServiceBusClientReceiver
    {
        private readonly object _lockObject;
        private readonly IServiceBusHostSettings _hostSettings;
        private ImmutableDictionary<string, ServiceBusReceiver> _receivers;

        public ServiceBusClientReceiver(IServiceBusHostSettings hostSettings)
        {
            _hostSettings = hostSettings ?? throw new ArgumentNullException(nameof(hostSettings));
            _lockObject = new object();
            _receivers = ImmutableDictionary<string, ServiceBusReceiver>.Empty;
        }

        public Task<IReadOnlyList<ServiceBusReceivedMessage>> ReceiveMessagesAsync(int maxMessages, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(string queueName, ServiceBusReceiverOptions options, out ServiceBusReceiver receiver)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(queueName)) throw new ArgumentNullException(nameof(queueName));
            
            receiver = default;

            lock (_lockObject)
            {
                if (_receivers.TryGetValue(queueName, out receiver))
                    return true;
            }

            lock (_lockObject)
            {
                receiver = _hostSettings.ServiceBusClient.CreateReceiver(queueName, options);
                _receivers = _receivers.Add(queueName, receiver);
            }

            return true;
        }

        public async ValueTask DisposeAsync()
        {
            lock (_lockObject)
            {
                if (_receivers.IsEmpty)
                    return;
            }

            foreach (var (topicName, serviceBusReceiver) in _receivers)
            {
                if (serviceBusReceiver.IsClosed)
                    continue;

                await serviceBusReceiver.DisposeAsync();
            }
        }
    }
}