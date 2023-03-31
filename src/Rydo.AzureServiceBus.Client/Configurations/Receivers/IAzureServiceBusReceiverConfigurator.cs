namespace Rydo.AzureServiceBus.Client.Configurations.Receivers
{
    using System;
    using System.Collections.Generic;

    public interface IAzureServiceBusReceiverConfigurator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="container"></param>
        void Configure(Type type, Action<IReceiverContextContainer> container);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="container"></param>
        void Configure(IEnumerable<Type> types, Action<IReceiverContextContainer> container);

        /// <summary>
        /// /
        /// </summary>
        /// <param name="type"></param>
        /// <param name="queueName"></param>
        /// <param name="container"></param>
        void Configure(Type type, string queueName, Action<IReceiverContextContainer> container);
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="queueName"></param>
        /// <param name="container"></param>
        void Configure(IEnumerable<Type> types, string queueName, Action<IReceiverContextContainer> container);
    }
}