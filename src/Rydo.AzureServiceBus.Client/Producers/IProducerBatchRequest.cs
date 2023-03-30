namespace Rydo.AzureServiceBus.Client.Producers
{
    using System.Collections.Generic;
    using Headers;

    public interface IProducerBatchRequest
    {
        /// <summary>
        /// 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The topic to produce the message to.
        /// </summary>
        string TopicName { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<ProducerRequest> Items { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageValue"></param>
        /// <param name="headers"></param>
        void Add(in object messageValue, MessageHeaders headers = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageKey"></param>
        /// <param name="messageValue"></param>
        /// <param name="headers"></param>
        void Add(in string messageKey, in object messageValue, MessageHeaders headers = default);

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="topicName"></param>
        // /// <param name="messageValue"></param>
        // /// <param name="headers"></param>
        // void AddRaw(string topicName, byte[] messageValue, MessageHeaders headers = default);
        //
        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="topicName"></param>
        // /// <param name="messageKey"></param>
        // /// <param name="messageValue"></param>
        // /// <param name="headers"></param>
        // void AddRaw(string topicName, string messageKey, byte[] messageValue, MessageHeaders headers = default);
    }
}