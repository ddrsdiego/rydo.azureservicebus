namespace Rydo.AzureServiceBus.Client.Headers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class Headers : IEnumerable<IHeader>
    {
        private readonly List<IHeader> _headers = new List<IHeader>();

        /// <summary>Append a new header to the collection.</summary>
        /// <param name="key">The header key.</param>
        /// <param name="val">
        ///     The header value (possibly null). Note: A null
        ///     header value is distinct from an empty header
        ///     value (array of length 0).
        /// </param>
        public void Add(string key, byte[] val) => Add(new Header(key, val));

        /// <summary>Append a new header to the collection.</summary>
        /// <param name="header">The header to add to the collection.</param>
        public void Add(Header header)
        {
            if (header.KeyIsInvalid)
                throw new ArgumentNullException("Meassage header key cannot be null.");
            
            if (header.HasInvalidValue)
                throw new ArgumentNullException("Message header value cannot be null or empty.");

            _headers.Add(header);
        }

        /// <summary>
        ///     Get the value of the latest header with the specified key.
        /// </summary>
        /// <param name="key">The key to get the associated value of.</param>
        /// <returns>
        ///     The value of the latest element in the collection with the specified key.
        /// </returns>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">
        ///     The key <paramref name="key" /> was not present in the collection.
        /// </exception>
        public byte[] GetLastBytes(string key)
        {
            if (TryGetLastBytes(key, out var lastHeader))
                return lastHeader;

            throw new KeyNotFoundException("The key " + key + " was not present in the headers collection.");
        }

        /// <summary>
        ///     Try to get the value of the latest header with the specified key.
        /// </summary>
        /// <param name="key">The key to get the associated value of.</param>
        /// <param name="lastHeader">
        ///     The value of the latest element in the collection with the
        ///     specified key, if a header with that key was present in the
        ///     collection.
        /// </param>
        /// <returns>
        ///     true if the a value with the specified key was present in
        ///     the collection, false otherwise.
        /// </returns>
        public bool TryGetLastBytes(string key, out byte[] lastHeader)
        {
            for (var index = _headers.Count - 1; index >= 0; --index)
            {
                if (_headers[index].Key != key)
                    continue;

                lastHeader = _headers[index].GetValueBytes();
                return true;
            }

            lastHeader = null;
            return false;
        }

        /// <summary>Removes all headers for the given key.</summary>
        /// <param name="key">The key to remove all headers for</param>
        public void Remove(string key) => _headers.RemoveAll((Predicate<IHeader>) (a => a.Key == key));

        /// <summary>
        ///     Returns an enumerator that iterates through the headers collection.
        /// </summary>
        /// <returns>
        ///     An enumerator object that can be used to iterate through the headers collection.
        /// </returns>
        public IEnumerator<IHeader> GetEnumerator() => new HeadersEnumerator(this);

        /// <summary>
        ///     Returns an enumerator that iterates through the headers collection.
        /// </summary>
        /// <returns>
        ///     An enumerator object that can be used to iterate through the headers collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new HeadersEnumerator(this);

        /// <summary>Gets the header at the specified index</summary>
        /// <param key="index">
        ///     The zero-based index of the element to get.
        /// </param>
        public IHeader this[int index] => _headers[index];

        /// <summary>The number of headers in the collection.</summary>
        public int Count => _headers.Count;

        internal class HeadersEnumerator : IEnumerator<IHeader>, IDisposable
        {
            private readonly Headers _headers;
            private int _location = -1;

            public HeadersEnumerator(Headers headers) => _headers = headers;

            public object Current => ((IEnumerator<IHeader>) this).Current;

            IHeader IEnumerator<IHeader>.Current => _headers._headers[_location];

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                ++_location;
                return _location < _headers._headers.Count;
            }

            public void Reset() => _location = -1;
        }
    }
}