namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Abstractions.Observers.Observables;
    using Azure.Messaging.ServiceBus;
    using Configurations.Host;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Handlers;
    using Middlewares;
    using Middlewares.Observers;
    using Middlewares.Observers.Observable;
    using Utils;

    internal sealed class ReceiverListener : IReceiverListener
    {
        private IServiceProvider _serviceProvider;
        private ServiceBusReceiver _receiver;
        private IMiddlewareExecutor _middlewareExecutor;

        private readonly Task _readerTask;
        private readonly ILogger<ReceiverListener> _logger;
        private readonly SubscriberContext _subscriberContext;
        private readonly CancellationToken _cancellationToken;
        private readonly Channel<ServiceBusReceivedMessage> _queue;
        private readonly ReceiveObservable _receiveObservable;
        private readonly FinishConsumerMiddlewareObservable _finishConsumerMiddlewareObservable;
        private readonly TaskCompletionSource<bool> _taskCompletion;

        internal ReceiverListener(ILogger<ReceiverListener> logger, IServiceBusClientWrapper serviceBusClient,
            SubscriberContext subscriberContext)
        {
            const int channelCapacity = 2_000;

            _subscriberContext = subscriberContext ?? throw new ArgumentNullException(nameof(subscriberContext));
            _taskCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            IsRunning = Task.FromResult(true);

            _logger = logger;
            _receiveObservable = new ReceiveObservable();
            _finishConsumerMiddlewareObservable = new FinishConsumerMiddlewareObservable();

            _cancellationToken = new CancellationToken();

            var channelOptions = new BoundedChannelOptions(subscriberContext.Specification.Consumer.BufferSize)
            {
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };
            BusClient = serviceBusClient;
            _queue = Channel.CreateBounded<ServiceBusReceivedMessage>(channelOptions);

            _readerTask = Task.Run(async () => await ReadFromChannel());
        }

        public IServiceBusClientWrapper BusClient { get; }

        public IReceiverListener ServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            return this;
        }

        public IReceiverListener MiddleExecutor(IMiddlewareExecutor middlewareExecutor)
        {
            _middlewareExecutor = middlewareExecutor;
            return this;
        }

        public Task<bool> IsRunning { get; set; }

        public IConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservable.Connect(observer);
        }

        public IConnectHandle ConnectFinishConsumerMiddlewareObserver(IFinishConsumerMiddlewareObserver observer)
        {
            return _finishConsumerMiddlewareObservable.Connect(observer);
        }

        public async Task<bool> StartAsync(CancellationToken stoppingToken)
        {
            await CreateReceiversAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                var receivedMessages =
                    await _receiver.ReceiveMessagesAsync(_subscriberContext.Specification.MaxMessages,
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

        public async Task<bool> StopAsync(CancellationToken stoppingToken)
        {
            try
            {
                _queue.Writer.Complete();

                await _taskCompletion.Task;
                await BusClient.DisposeAsync();
                _readerTask.Dispose();

                IsRunning = _taskCompletion.Task;

                return await IsRunning;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task CreateReceiversAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _receiveObservable.PreStartReceive(_subscriberContext);

                var options = new ServiceBusReceiverOptions
                {
                    PrefetchCount = _subscriberContext.Specification.PrefetchCount,
                    Identifier = _subscriberContext.Specification.SubscriptionName,
                    ReceiveMode = _subscriberContext.Specification.ReceiveMode
                };

                if (!BusClient.Receiver.TryGet(_subscriberContext.TopicSubscriptionName, options,
                        out _receiver))
                {
                }

                await _receiveObservable.PostStartReceive(_subscriberContext);
            }
            catch (Exception e)
            {
                await _receiveObservable.FaultStartReceive(_subscriberContext, e);
                await StopAsync(stoppingToken);
            }
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
            var batchCapacity = _subscriberContext.Specification.Consumer.BufferSize;

            try
            {
                while (await _queue.Reader.WaitToReadAsync())
                {
                    var counter = 0;

                    var messageConsumerContext =
                        new MessageConsumerContext(_subscriberContext, _receiver, _cancellationToken);

                    while (counter < batchCapacity && _queue.Reader.TryRead(out var receivedMessage))
                    {
                        var messageContext = new MessageContext(receivedMessage);
                        messageConsumerContext.Add(messageContext);

                        if (_receiveObservable.Count >= 0)
                            await _receiveObservable.PreReceiveAsync(messageContext);

                        counter++;
                    }

                    await _middlewareExecutor.Execute(_serviceProvider.CreateScope(), messageConsumerContext,
                        _ => Task.CompletedTask);

                    if (_finishConsumerMiddlewareObservable.Count > 0)
                        await _finishConsumerMiddlewareObservable.EndConsumerAsync(messageConsumerContext);
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
            finally
            {
                _taskCompletion.TrySetResult(false);
            }
        }
    }
}