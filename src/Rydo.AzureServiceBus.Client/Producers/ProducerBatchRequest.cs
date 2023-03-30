namespace Rydo.AzureServiceBus.Client.Producers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Headers;

    public readonly struct ProducerBatchRequest : IProducerBatchRequest
    {
        private readonly object _syncObject;
        private readonly LinkedList<ProducerRequest> _items;

        private ProducerBatchRequest(string topicName)
        {
            _syncObject = new object();
            _items = new LinkedList<ProducerRequest>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IProducerBatchRequest Create() => new ProducerBatchRequest(string.Empty);

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="topicName">The topic the record will be appended to.</param>
        // /// <exception cref="ArgumentNullException"></exception>
        // internal static IProducerBatchRequest Create(string topicName) => new ProducerBatchRequest(topicName);

        public int Count => _items.Count;

        public string TopicName => _items.First.Value.TopicName;

        public IEnumerable<ProducerRequest> Items => _items.ToImmutableList();

        public void Add(in object messageValue, MessageHeaders headers = default) => 
            Add(Guid.NewGuid().ToString(), messageValue, headers);

        public void Add(in string messageKey, in object messageValue, MessageHeaders headers = default)
        {
            if (string.IsNullOrEmpty(messageKey)) throw new ArgumentNullException(nameof(messageValue));
            
            if (messageValue == null) throw new ArgumentNullException(nameof(messageValue));
            
            if (!messageValue.TryExtractTopicName(out var topicName))
                throw new ArgumentNullException(nameof(messageValue));

            headers ??= MessageHeaders.GetInstance();
            InternalAdd(ProducerRequest.GetInstance(topicName, messageKey, messageValue, headers));
        }

        // public void AddRaw(string topicName, byte[] messageValue, MessageHeaders headers = default)
        // {
        //     if (messageValue == null) throw new ArgumentNullException(nameof(messageValue));
        //
        //     headers ??= MessageHeaders.GetInstance();
        //     InternalAdd(ProducerRequest.GetInstance(topicName, messageValue, headers));
        // }
        //
        // public void AddRaw(string topicName, string messageKey, byte[] messageValue, MessageHeaders headers = default)
        // {
        //     if (messageKey == null) throw new ArgumentNullException(nameof(messageKey));
        //
        //     if (messageValue == null) throw new ArgumentNullException(nameof(messageValue));
        //
        //     headers ??= MessageHeaders.GetInstance();
        //     InternalAdd(ProducerRequest.GetInstance(topicName, messageKey, messageValue, headers));
        // }

        private void InternalAdd(ProducerRequest item)
        {
            lock (_syncObject)
            {
                _items.AddLast(item);
            }
        }
    }
}