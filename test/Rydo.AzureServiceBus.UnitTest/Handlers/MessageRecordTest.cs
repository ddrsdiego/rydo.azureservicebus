namespace Rydo.AzureServiceBus.UnitTest.Handlers
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using NUnit.Framework;

    public class MessageRecordTest
    {
        [Test]
        public void Test()
        {
            // ServiceBusReceivedMessage
            // var messageRecord = new MessageRecord();
        }
    }

    public interface IReceiveContext
    {
    }

    public interface IServiceBusReceiveContext :
        IReceiveContext
    {
        Task CompleteMessageAsync();
    }

    public class ServiceBusReceiveContext :
        IServiceBusReceiveContext
    {
        private readonly ServiceBusReceiver _receiver;

        public ServiceBusReceiveContext(ServiceBusReceiver receiver)
        {
            _receiver = receiver;
        }

        public Task CompleteMessageAsync()
        {
            throw new NotImplementedException();
        }
    }
}