namespace Rydo.AzureServiceBus.UnitTest.Headers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Client.Headers;
    using FluentAssertions;
    using NUnit.Framework;

    public class HeadersTest
    {
        [Test]
        public void Should_Get_HeaderValue_From_Key()
        {
            const string headerValue = "unit-test";
            const string headerKey = "header-key";

            var headers = new Headers();
            var header = new Header(headerKey, Encoding.UTF8.GetBytes(headerValue));

            headers.Add(header);

            var value = headers.GetLastBytes(headerKey);
            Encoding.UTF8.GetString(value).Should().Be(headerValue);
        }

        [Test]
        public void Should_Throws_KeyNotFoundException_When_Key_Is_Invalid()
        {
            const string headerValue = "unit-test";
            const string headerKey = "header-key";

            var headers = new Headers();
            var header = new Header(headerKey, Encoding.UTF8.GetBytes(headerValue));

            headers.Add(header);

            Assert.Throws<KeyNotFoundException>(() => headers.GetLastBytes("headerKey-wrong"));
        }
        
        [Test]
        public void Should_Throws_ArgumentNullException_When_Value_Is_Invalid()
        {
            const string val = "unit-test";

            var headers = new Headers();
            var header = new Header("header-key", Array.Empty<byte>());

            Assert.Throws<ArgumentNullException>(() => headers.Add(header));
        }

        [Test]
        public void Should_Throws_ArgumentNullException_When_Header_Is_Invalid_1()
        {
            const string val = "unit-test";

            var headers = new Headers();
            var header = new Header(string.Empty, Encoding.UTF8.GetBytes(val));

            Assert.Throws<ArgumentNullException>(() => headers.Add(header));
        }

        [Test]
        public void Should_Throws_ArgumentNullException_When_Key_Is_Invalid()
        {
            const string val = "unit-test";
            var headers = new Headers();
            Assert.Throws<ArgumentNullException>(() => headers.Add(string.Empty, Encoding.UTF8.GetBytes(val)));
        }
    }
}