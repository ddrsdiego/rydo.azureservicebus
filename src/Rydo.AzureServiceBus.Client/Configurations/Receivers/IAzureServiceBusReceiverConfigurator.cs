namespace Rydo.AzureServiceBus.Client.Configurations.Receivers
{
    using System;

    public interface IAzureServiceBusReceiverConfigurator
    {
        /// <summary>
        /// Configures a listener to the context having as a parameter the topic name assigned to the consumer handler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Configure<T>();
        
        /// <summary>
        /// Configures a listener to the context having as a parameter the topic name assigned to the consumer handler.
        /// </summary>
        /// <param name="container"></param>
        /// <typeparam name="T"></typeparam>
        void Configure<T>(Action<IReceiverContextContainer> container);
    }
}