namespace Rydo.AzureServiceBus.Client.Consumers.MessageRecordModel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Observers;
    using Observers.Observables;
    using Subscribers;
    using Serialization;
    using Utils;

    internal interface IMessageRecordAdapter :
        IMessageRecordAdapterObserverConnector
    {
        ValueTask<MessageRecord> ToMessageRecord(IMessageContext messageContext,
            Type contractType, CancellationToken cancellationToken = default);
    }

    internal sealed class MessageRecordAdapter : IMessageRecordAdapter
    {
        private readonly ISerializer _serializer;
        private readonly MessageRecordAdapterObservable _observable;

        public MessageRecordAdapter(ISerializer serializer)
        {
            _serializer = serializer;
            _observable = new MessageRecordAdapterObservable();
        }

        public IConnectHandle ConnectMessageRecordAdapterObserver(IMessageRecordAdapterObserver observer) =>
            _observable.Connect(observer);

        public async ValueTask<MessageRecord> ToMessageRecord(IMessageContext messageContext,
            Type contractType, CancellationToken cancellationToken = default)
        {
            var context = messageContext as MessageContext;

            MessageRecord messageRecord;

            try
            {
                await _observable.PreAdapter(messageContext, contractType);

                var messageValue = await _serializer.DeserializeAsync(context.Message.Body.GetBytes(), contractType,
                    cancellationToken);
                messageRecord =
                    MessageRecord.GetInstance(messageValue, context.Message);
            }
            catch (Exception e)
            {
                messageRecord =
                    MessageRecord.GetInvalidInstance(context.Message);

                await _observable.FaultAdapter(messageContext, contractType, e);
            }

            return messageRecord;
        }
    }
}