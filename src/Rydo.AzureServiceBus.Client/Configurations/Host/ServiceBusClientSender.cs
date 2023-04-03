namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using System;
    using System.Collections.Immutable;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;

    public interface IServiceBusClientSender : IAsyncDisposable
    {
        bool TryGet(string queueName, ServiceBusSenderOptions options, out ServiceBusSender sender);
    }

    internal sealed class ServiceBusClientSender : IServiceBusClientSender
    {
        private readonly object _lockObject;
        private readonly IServiceBusHostSettings _hostSettings;
        private ImmutableDictionary<string, ServiceBusSender> _senders;

        public ServiceBusClientSender(IServiceBusHostSettings hostSettings)
        {
            _hostSettings = hostSettings ?? throw new ArgumentNullException(nameof(hostSettings));
            _lockObject = new object();
            _senders = ImmutableDictionary<string, ServiceBusSender>.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(string queueName, ServiceBusSenderOptions options, out ServiceBusSender sender)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(queueName)) throw new ArgumentNullException(nameof(queueName));

            sender = default;

            lock (_lockObject)
            {
                if (_senders.TryGetValue(queueName, out sender))
                    return true;
            }

            lock (_lockObject)
            {
                if (_senders.TryGetValue(queueName, out sender))
                    return true;

                sender = _hostSettings.ServiceBusClient.CreateSender(queueName, options);
                _senders = _senders.Add(queueName, sender);
            }

            return true;
        }

        public async ValueTask DisposeAsync()
        {
            if (_senders.IsEmpty)
                return;

            foreach (var (topicName, serviceBusSender) in _senders)
            {
                if (serviceBusSender.IsClosed)
                    continue;

                await serviceBusSender.DisposeAsync();
            }
        }
    }
}