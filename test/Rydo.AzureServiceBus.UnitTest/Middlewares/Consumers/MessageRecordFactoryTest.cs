namespace Rydo.AzureServiceBus.UnitTest.Middlewares.Consumers
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Client.Consumers.MessageRecordModel;
    using Client.Consumers.MessageRecordModel.Observers;
    using Client.Consumers.Subscribers;
    using Client.Serialization;
    using Consumer.Models;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NUnit.Framework;

    public class MessageRecordFactoryTest
    {
        private readonly ISerializer _serializer;
        private readonly IServiceBusMessageContext _serviceBusMessageContext;

        public MessageRecordFactoryTest()
        {
            _serviceBusMessageContext = Substitute.For<IServiceBusMessageContext>();
            _serializer = new SystemTextJsonSerializer(Substitute.For<ILogger<SystemTextJsonSerializer>>());
        }

        [Test]
        public async Task Should_Fail_To_Adapter_ServiceBusMessageReceived_To_MessageRecord()
        {
            //arrange

            _serviceBusMessageContext.Body.Returns(new ServiceBusMessageBody(new BinaryData(Array.Empty<byte>())));
            var messageContext = new MessageContext(_serviceBusMessageContext);

            //act
            var sut = new MessageRecordAdapter(_serializer);

            sut.ConnectMessageRecordAdapterObserver(
                new LogMessageRecordAdapterObserver(Substitute.For<ILoggerFactory>()));
            sut.ConnectMessageRecordAdapterObserver(new MetricsMessageRecordAdapterObserver());
            var messageRecord = await sut.ToMessageRecord(messageContext, typeof(AccountCreated));

            //assert

            messageRecord.IsValid.Should().BeFalse();
            messageRecord.IsInvalid.Should().BeTrue();
        }

        [Test]
        public async Task Should_Adapter_ServiceBusMessageReceived_To_MessageRecord()
        {
            //arrange

            var accountCreated = new AccountCreated("5090016");
            var data = JsonSerializer.SerializeToUtf8Bytes(accountCreated);
            _serviceBusMessageContext.Body.Returns(new ServiceBusMessageBody(new BinaryData(data)));
            var messageContext = new MessageContext(_serviceBusMessageContext);

            //act
            var sut = new MessageRecordAdapter(_serializer);
            var messageRecord = await sut.ToMessageRecord(messageContext, typeof(AccountCreated));

            //assert
            messageRecord.IsValid.Should().BeTrue();
        }
    }
}