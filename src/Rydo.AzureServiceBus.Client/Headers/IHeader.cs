namespace Rydo.AzureServiceBus.Client.Headers
{
    using System;

    public interface IHeader
    {
        /// <summary>The header key.</summary>
        string Key { get; }

        bool KeyIsInvalid { get; }
        
        /// <summary>The serialized header value data.</summary>
        byte[] GetValueBytes();
        
        bool HasValidValue { get; }
        
        bool HasInvalidValue { get; }
    }

    public class Header : IHeader
    {
        private readonly byte[] _val;

        /// <summary>The header key.</summary>
        public string Key { get; private set; }

        public bool KeyIsInvalid => string.IsNullOrWhiteSpace(Key);

        /// <summary>Get the serialized header value data.</summary>
        public byte[] GetValueBytes() => _val;

        public bool HasValidValue
        {
            get
            {
                var valInBytes = GetValueBytes();
                return valInBytes != null && valInBytes.Length > 0;
            }
        }

        public bool HasInvalidValue => !HasValidValue;

        /// <summary>Create a new Header instance.</summary>
        /// <param name="key">The header key.</param>
        /// <param name="value">The header value (may be null).</param>
        public Header(string key, byte[] value)
        {
            Key = key ?? throw new ArgumentNullException("Message header key cannot be null.");
            _val = value;
        }
    }
}