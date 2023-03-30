namespace Rydo.AzureServiceBus.Client.Headers
{
    using System.Collections.Generic;

    public interface IMessageHeaders : IEnumerable<KeyValuePair<string, byte[]>>
    {
        /// <summary>
        /// Gets or sets the header with a specified key
        /// </summary>
        /// <param name="key">The zero-based index of the element to get</param>
        byte[] this[string key] { get; set; }

        void Remove(string key);
    }
}