namespace Rydo.AzureServiceBus.Client.Configurations.Receivers.Extensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Consumers.Subscribers;
    using Logging.Observers;
    using Metrics.Observers;
    using Middlewares.Observers;

    internal static class ReceiverListenerObserverEx
    {
        public static void ConnectObservers(this IReceiverListener receiverListener, IServiceProvider sp)
        {
            var logger = sp.GetRequiredService<ILoggerFactory>();
            
            receiverListener.ConnectReceiveObserver(new PrometheusIncomingReceiveObserver());
            receiverListener.ConnectReceiveObserver(new LogReceiveObserver(logger));
            receiverListener.ConnectFinishConsumerMiddlewareObserver(new LogFinishConsumerMiddlewareObserver(logger));
        }
    }
}