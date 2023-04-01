namespace Rydo.AzureServiceBus.Client.Configurations.Receivers.Extensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Rydo.AzureServiceBus.Client.Consumers.Subscribers;
    using Rydo.AzureServiceBus.Client.Logging.Observers;
    using Rydo.AzureServiceBus.Client.Metrics.Observers;

    internal static class ReceiverListenerObserverEx
    {
        public static void ConnectObservers(this IReceiverListener receiverListener, IServiceProvider sp)
        {
            var logLoggingReceiveObserver = sp.GetRequiredService<ILogger<LogReceiveObserver>>();

            receiverListener.ConnectReceiveObserver(new PrometheusIncomingReceiveObserver());
            receiverListener.ConnectReceiveObserver(new LogReceiveObserver(logLoggingReceiveObserver));
        }
    }
}