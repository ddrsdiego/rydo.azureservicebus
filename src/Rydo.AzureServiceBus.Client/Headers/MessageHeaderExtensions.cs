namespace Rydo.AzureServiceBus.Client.Headers
{
    using System.Text;

    public static class MessageHeaderExtensions
    {
        /// <summary>
        /// Set a key / value in the header that you want to be sent in the message
        /// </summary>
        /// <param name="headers">An instance of the message header</param>
        /// <param name="key">Unique key in the header</param>
        /// <param name="value">Desired value to send in header</param>
        public static void SetString(this IMessageHeaders headers, string key, string value) =>
            headers.SetString(key, value, Encoding.UTF8);

        internal static string GetString(this IMessageHeaders headers, string key) =>
            headers.GetString(key, Encoding.UTF8);

        // internal static void Remove(this IMessageHeaders headers, string key) => headers.Remove(key);

        internal static IMessageHeaders SetPartition(this IMessageHeaders headers, string partitionKey)
        {
            headers.SetString(MessageHeadersDefault.PartitionKey, partitionKey);
            return headers;
        }

        private static string GetString(this IMessageHeaders headers, string key, Encoding encoding)
        {
            var value = headers[key];
            return value != null ? encoding.GetString(headers[key]) : null;
        }

        private static void SetString(this IMessageHeaders headers, string key, string value, Encoding encoding) =>
            headers[key] = encoding.GetBytes(value);
    }
}