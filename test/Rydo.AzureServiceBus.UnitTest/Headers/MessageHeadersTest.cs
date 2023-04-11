namespace Rydo.AzureServiceBus.UnitTest.Headers
{
    using System.Linq;
    using System.Text;
    using Client.Headers;
    using FluentAssertions;
    using NUnit.Framework;

    public class MessageHeadersTest
    {
        private const string Key = "abc";
        private const string StrValue = "123";
        private readonly byte[] _value = Encoding.UTF8.GetBytes("123");

        [Test]
        public void Add_WithKeyNotNull_ShouldAddValueCorrectly()
        {
            // Arrange
            var header = MessageHeaders.GetInstance();

            // Act
            header.Add(Key, _value);

            // Assert
            header[Key].Should().BeEquivalentTo(_value);
        }

        [Test]
        public void GetHeaders_ShouldReturnHeaders()
        {
            // Arrange
            var headers = new Headers {{Key, _value}};
            var messageHeaders = new MessageHeaders(headers);

            // Act
            var result = messageHeaders.GetHeaders();

            // Assert
            result.Should().BeEquivalentTo(headers);
        }

        [Test]
        public void SetString_WithValueNotNull_ShouldAddValueCorrectly()
        {
            // Arrange
            var header = MessageHeaders.GetInstance();

            // Act
            header.SetString(Key, StrValue);

            // Assert
            header.GetString(Key).Should().BeEquivalentTo(StrValue);
        }
        
        [Test]
        public void Test()
        {
            const string headerValue = "unit-test";
            const string headerKey1 = "header-key-1";
            const string headerKey2 = "header-key-2";

            var headerValueInBytes = Encoding.UTF8.GetBytes(headerValue);
            var headers = new Headers
            {
                new Header(headerKey1, headerValueInBytes),
                new Header(headerKey2, headerValueInBytes)
            };

            var sut = new MessageHeaders(headers);
            foreach (var (key, value) in sut)
            {
            }

            var firstHeader = sut.First(x => x.Key.Equals(headerKey1));
            firstHeader.Key.Should().Be(headerKey1);
        }
    }
}