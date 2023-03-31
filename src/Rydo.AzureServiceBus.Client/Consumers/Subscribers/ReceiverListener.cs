namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Handlers;
    using Middlewares;

    internal sealed class ReceiverListener : IReceiverListener
    {
        private ILogger<ReceiverListener> _logger;
        private IServiceProvider _serviceProvider;
        private ServiceBusClient _serviceBusClient;
        private ServiceBusReceiver _receiver;
        private IMiddlewareExecutor _middlewareExecutor;

        private readonly Task _readerTask;
        private readonly SubscriberContext _subscriberContext;
        private readonly CancellationToken _cancellationToken;
        private readonly Channel<ServiceBusReceivedMessage> _queue;

        internal ReceiverListener(SubscriberContext subscriberContext)
        {
            const int channelCapacity = 2_000;

            _subscriberContext = subscriberContext ?? throw new ArgumentNullException(nameof(subscriberContext));
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

        public IReceiverListener ServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<ReceiverListener>>();

            return this;
        }

        public IReceiverListener MiddleExecutor(IMiddlewareExecutor middlewareExecutor)
        {
            _middlewareExecutor = middlewareExecutor;
            return this;
        }

        public IReceiverListener ServiceBusClient(ServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient));
            return this;
        }

        public Task<bool> IsRunning { get; set; }

        public async Task<bool> StartAsync(CancellationToken stoppingToken)
        {
            CreateReceiver();

            while (!stoppingToken.IsCancellationRequested)
            {
                var receivedMessages =
                    await _receiver.ReceiveMessagesAsync(_subscriberContext.SubscriberSpecification.MaxDelivery,
                        cancellationToken: stoppingToken);

                if (receivedMessages == null || receivedMessages.Count == 0)
                    continue;

                var messages = receivedMessages.ToArray();
                for (var index = 0; index < messages.Length; index++)
                {
                    await EnqueueAsync(messages[index], stoppingToken);
                }
            }

            return IsRunning.Result;
        }

        private void CreateReceiver()
        {
            _receiver ??= _serviceBusClient.CreateReceiver(_subscriberContext.QueueSubscription,
                new ServiceBusReceiverOptions
                {
                    PrefetchCount = _subscriberContext.SubscriberSpecification.MaxDelivery,
                    Identifier = _subscriberContext.SubscriberSpecification.SubscriptionName,
                    ReceiveMode = ServiceBusReceiveMode.PeekLock
                });
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

                    var messageConsumerContext =
                        new MessageConsumerContext(_subscriberContext, _receiver, _cancellationToken);

                    while (counter < batchCapacity && _queue.Reader.TryRead(out var receivedMessage))
                    {
                        var messageContext = new MessageContext(receivedMessage);
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
}