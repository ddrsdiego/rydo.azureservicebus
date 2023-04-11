namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public interface IServiceBusClientWrapper : IAsyncDisposable
    {
        IServiceBusClientAdmin Admin { get; }
        IServiceBusClientSender Sender { get; }
        IServiceBusClientReceiver Receiver { get; }
    }

    internal sealed class ServiceBusClientWrapper : IServiceBusClientWrapper
    {
        public ServiceBusClientWrapper(ILoggerFactory logger, IServiceBusHostSettings hostSettings)
        {
            if (hostSettings == null) throw new ArgumentNullException(nameof(hostSettings));

            Admin = new ServiceBusClientAdmin(hostSettings);
            Sender = new ServiceBusClientSender(hostSettings);
            Receiver = new ServiceBusClientReceiver(logger.CreateLogger<ServiceBusClientReceiver>(), hostSettings);
        }

        public IServiceBusClientAdmin Admin { get; }

        public IServiceBusClientSender Sender { get; }

        public IServiceBusClientReceiver Receiver { get; }

        public async ValueTask DisposeAsync()
        {
            await Admin.DisposeAsync();
            await Sender.DisposeAsync();
            await Receiver.DisposeAsync();
        }
    }
}