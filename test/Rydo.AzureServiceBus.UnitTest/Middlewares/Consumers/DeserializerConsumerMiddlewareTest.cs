namespace Rydo.AzureServiceBus.UnitTest.Middlewares.Consumers
{
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Client.Configurations.Host;
    using Client.Consumers.MessageRecordModel;
    using Client.Consumers.Subscribers;
    using Client.Handlers;
    using Client.Middlewares.Consumers;
    using Consumer.ConsumerHandlers;
    using Consumer.Models;
    using NSubstitute;
    using NUnit.Framework;

    public class DeserializerConsumerMiddlewareTest
    {
        private readonly IServiceBusMessageContext _serviceBusMessageContext;
        private readonly IMessageRecordAdapter _messageRecordAdapter;
        private readonly IServiceBusClientReceiver _serviceBusClientReceiver;

        public DeserializerConsumerMiddlewareTest()
        {
            _messageRecordAdapter = Substitute.For<IMessageRecordAdapter>();
            _serviceBusClientReceiver = Substitute.For<IServiceBusClientReceiver>();
            _serviceBusMessageContext = Substitute.For<IServiceBusMessageContext>();
        }

        [Test]
        public async Task Should_Deserialize_Message()
        {
            //arrange
            var accountCreated = new AccountCreated("5090016");
            var data = JsonSerializer.SerializeToUtf8Bytes(accountCreated);

            var builder = new SubscriberConfiguratorBuilder("topic-name", "subscription-name");
            var configurator = builder.Build();

            var subscriberContext = new SubscriberContext(new SubscriberSpecification(configurator.Value),
                typeof(AccountCreated), typeof(AccountCreatedConsumerHandler));

            var messageConsumerContext =
                new MessageConsumerContext(subscriberContext, _serviceBusClientReceiver,
                    CancellationToken.None);

            _serviceBusMessageContext.Body.Returns(new ServiceBusMessageBody(new BinaryData(data)));

            _messageRecordAdapter
                .ToMessageRecord(Arg.Any<IMessageContext>(), Arg.Any<Type>(), Arg.Any<CancellationToken>())
                .Returns(x => MessageRecord.GetInstance(accountCreated, _serviceBusMessageContext));


            messageConsumerContext.Add(new MessageContext(_serviceBusMessageContext));
            var sut = new DeserializerConsumerMiddleware(_messageRecordAdapter);

            //act
            await sut.InvokeAsync(messageConsumerContext, nextContext => Task.CompletedTask);

            //assert
            await _messageRecordAdapter.Received(1).ToMessageRecord(Arg.Any<IMessageContext>(), Arg.Any<Type>(),
                Arg.Any<CancellationToken>());
        }
    }
}