namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.IO;

    public interface IMessageBody
    {
        long? Length { get; }

        /// <summary>
        /// Return the message body as a stream
        /// </summary>
        /// <returns></returns>
        Stream GetStream();

        /// <summary>
        /// Return the message body as a byte array
        /// </summary>
        /// <returns></returns>
        byte[] GetBytes();

        /// <summary>
        /// Return the message body as a string
        /// </summary>
        /// <returns></returns>
        string GetString();
    }

    public sealed class ServiceBusMessageBody :
        IMessageBody
    {
        private readonly BinaryData _data;

        public ServiceBusMessageBody(BinaryData data)
        {
            _data = data;
        }
        
        public long? Length => _data.ToMemory().Length;
        
        public Stream GetStream() => _data.ToStream();

        public byte[] GetBytes() => _data.ToArray();

        public string GetString() => _data.ToString();
    }
}