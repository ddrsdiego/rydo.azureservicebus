namespace Rydo.AzureServiceBus.UnitTest.Headers
{
    using System.Linq;
    using System.Text;
    using Client.Headers;
    using FluentAssertions;
    using NUnit.Framework;

    public class MessageHeadersTest
    {
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