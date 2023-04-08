namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
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
        private IMiddlewareExecutor _middlewareExecutor;

        private readonly Task _readerTask;
        private readonly SubscriberContext _subscriberContext;
        private readonly CancellationToken _cancellationToken;
        private readonly ReceiveObservable _receiveObservable;
        private readonly Channel<ServiceBusMessageContext> _queue;
        private readonly TaskCompletionSource<bool> _taskCompletion;
        private readonly IServiceBusClientAdmin _serviceBusClientAdmin;
        private readonly IServiceBusClientReceiver _serviceBusClientReceiver;
        private readonly FinishConsumerMiddlewareObservable _finishConsumerMiddlewareObservable;

        internal ReceiverListener(IServiceBusClientWrapper serviceBusClientWrapper, SubscriberContext subscriberContext)
        {
            _subscriberContext = subscriberContext ?? throw new ArgumentNullException(nameof(subscriberContext));

            _serviceBusClientAdmin = serviceBusClientWrapper.Admin;
            _serviceBusClientReceiver = serviceBusClientWrapper.Receiver;

            _taskCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            IsRunning = Task.FromResult(true);
            IsStopped = Task.FromResult(false);

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

            _queue = Channel.CreateBounded<ServiceBusMessageContext>(channelOptions);

            _readerTask = Task.Run(async () => await ReadFromChannel());
        }

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

        public Task<bool> IsStopped { get; set; }

        public IConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservable.Connect(observer);
        }

        public IConnectHandle ConnectFinishConsumerMiddlewareObserver(IFinishConsumerMiddlewareObserver observer)
        {
            return _finishConsumerMiddlewareObservable.Connect(observer);
        }

        public Task CreateEntitiesIfNotExistAsync(SubscriberContext subscriberContext, CancellationToken stoppingToken)
            => _serviceBusClientAdmin.CreateEntitiesIfNotExistAsync(subscriberContext, stoppingToken);

        public async Task<bool> StartAsync(CancellationToken stoppingToken)
        {
            await CreateReceiversAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                await foreach (var receivedMessages in _serviceBusClientReceiver.StartConsumerAsync(
                                   cancellationToken: stoppingToken))
                {
                    if (receivedMessages == null) continue;
                    await EnqueueAsync(receivedMessages, stoppingToken);
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
                await _serviceBusClientReceiver.DisposeAsync();

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

                _serviceBusClientReceiver.TryCreateReceiver(_subscriberContext.Specification.QueueName, options);

                await _receiveObservable.PostStartReceive(_subscriberContext);
            }
            catch (Exception e)
            {
                await _receiveObservable.FaultStartReceive(_subscriberContext, e);
                await StopAsync(stoppingToken);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ValueTask EnqueueAsync(ServiceBusMessageContext receivedMessage, CancellationToken cancellationToken)
        {
            return _queue.Writer.WriteAsync(receivedMessage, cancellationToken);
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
                        new MessageConsumerContext(_subscriberContext, _serviceBusClientReceiver, _cancellationToken);

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