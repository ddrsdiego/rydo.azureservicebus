namespace Rydo.AzureServiceBus.Client.Headers
{
    using System.Collections;
    using System.Collections.Generic;

    public sealed class MessageHeaders : IMessageHeaders
    {
        private readonly Headers _headers;

        internal MessageHeaders(Headers headers) => _headers = headers;

        internal MessageHeaders(IReadOnlyDictionary<string, object> applicationProperties)
        {
            foreach (var header in applicationProperties)
            {
                this.SetString(header.Key, header.Value.ToString());
            }
        }

        internal static MessageHeaders GetInstance() => new MessageHeaders(new Headers());

        public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator()
        {
            if (_headers == null) yield break;

            foreach (var header in _headers)
            {
                yield return new KeyValuePair<string, byte[]>(header.Key, header.GetValueBytes());
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public byte[] this[string key]
        {
            get => _headers != null && _headers.TryGetLastBytes(key, out var value) ? value : null;
            set
            {
                _headers?.Remove(key);
                _headers?.Add(key, value);
            }
        }

        public void Remove(string key) => _headers?.Remove(key);

        // public Headers GetHeaders() => _headers;
    }
}