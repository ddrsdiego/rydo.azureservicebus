﻿namespace Rydo.AzureServiceBus.Client.Consumers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Middlewares;

    public sealed class Subscriber : ISubscriber
    {
        private ILogger<Subscriber> _logger;
        private IServiceProvider _serviceProvider;
        private ServiceBusClient _serviceBusClient;
        private ServiceBusReceiver _receiver;
        private IMiddlewareExecutor _middlewareExecutor;

        private readonly Task _readerTask;
        private readonly ConsumerContext _consumerContext;
        private readonly CancellationToken _cancellationToken;
        private readonly Channel<ServiceBusReceivedMessage> _queue;

        internal Subscriber(ConsumerContext consumerContext)
        {
            const int channelCapacity = 2_000;

            _consumerContext = consumerContext ?? throw new ArgumentNullException(nameof(consumerContext));
            IsRunning = Task.FromResult(true);

            _cancellationToken = new CancellationToken();
            var channelOptions = new BoundedChannelOptions(channelCapacity)
            {
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            _queue = Channel.CreateBounded<ServiceBusReceivedMessage>(channelOptions);

            _readerTask = Task.Run(ReadFromChannel);
        }

        public ISubscriber WithServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            return this;
        }

        public ISubscriber WithMiddleExecutor(IMiddlewareExecutor middlewareExecutor)
        {
            _middlewareExecutor = middlewareExecutor;
            return this;
        }

        public ISubscriber WithServiceBusClient(ServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient));
            return this;
        }

        public ISubscriber WithLogging(ILogger<Subscriber> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }

        public Task<bool> IsRunning { get; set; }

        public async Task<bool> StartAsync(CancellationToken stoppingToken)
        {
            _receiver ??= _serviceBusClient.CreateReceiver(_consumerContext.ConsumerSpecification.TopicName,
                _consumerContext.ConsumerSpecification.SubscriptionName);

            var entityPath = _receiver.EntityPath;

            while (!stoppingToken.IsCancellationRequested)
            {
                var receivedMessages = await _receiver.ReceiveMessagesAsync(1_000, cancellationToken: stoppingToken);
                if (receivedMessages == null && receivedMessages.Count == 0)
                    continue;

                foreach (var receivedMessage in receivedMessages)
                {
                    await EnqueueAsync(receivedMessage, stoppingToken);
                }
            }

            return IsRunning.Result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Task EnqueueAsync(ServiceBusReceivedMessage receivedMessage,
            CancellationToken cancellationToken = default)
        {
            var writeTask = _queue.Writer.WriteAsync(receivedMessage, cancellationToken);
            return writeTask.IsCompletedSuccessfully ? Task.CompletedTask : SlowWrite(writeTask);

            static async Task SlowWrite(ValueTask task) => await task;
        }

        private async Task ReadFromChannel()
        {
            const int batchCapacity = 1_000;
            try
            {
                while (await _queue.Reader.WaitToReadAsync(_cancellationToken))
                {
                    var counter = 0;

                    var messageConsumerContext = new MessageConsumerContext(_consumerContext, _receiver);

                    while (counter < batchCapacity && _queue.Reader.TryRead(out var receivedMessage))
                    {
                        var messageId = receivedMessage.MessageId;
                        var partitionKey = receivedMessage.PartitionKey;
                        var payload = receivedMessage.Body.ToArray();

                        var message = new MessageReceived(messageId, partitionKey, payload);
                        var messageContext = new MessageContext(message, receivedMessage);

                        messageConsumerContext.Add(messageContext);
                        counter++;
                    }

                    await _middlewareExecutor.Execute(_serviceProvider.CreateScope(), messageConsumerContext,
                        _ => Task.CompletedTask);
                }
            }
            catch (OperationCanceledException e) when (e.CancellationToken == _cancellationToken)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public sealed class MessageContext
    {
        private MessageConsumerContext _messageConsumerContext;

        public MessageContext(MessageReceived message, ServiceBusReceivedMessage receivedMessage)
        {
            Message = message;
            ReceivedMessage = receivedMessage;
        }

        public readonly MessageReceived Message;
        public MessageRecord MessageRecord;
        public readonly ServiceBusReceivedMessage ReceivedMessage;

        internal void SetMessageRecord(MessageRecord messageRecord)
        {
            MessageRecord = messageRecord;
            MessageRecord.SetMessageConsumerContext(_messageConsumerContext);
        }

        internal void SetMessageConsumerContext(MessageConsumerContext context)
        {
            _messageConsumerContext = context ?? throw new ArgumentNullException(nameof(context));
        }
    }

    public readonly struct MessageReceived
    {
        public MessageReceived(string messageId, string partitionKey, ReadOnlyMemory<byte> payload)
        {
            MessageId = messageId;
            PartitionKey = partitionKey;
            Payload = payload;
        }

        public readonly string MessageId;
        public readonly string PartitionKey;
        public readonly ReadOnlyMemory<byte> Payload;
    }
}