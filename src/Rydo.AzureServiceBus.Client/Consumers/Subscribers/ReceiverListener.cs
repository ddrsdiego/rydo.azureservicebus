namespace Rydo.AzureServiceBus.Client.Consumers.Subscribers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Abstractions.Observers;
    using Abstractions.Observers.Observables;
    using Configurations.Host;
    using Microsoft.Extensions.DependencyInjection;
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
        private readonly Channel<IServiceBusMessageContext> _messagesBuffer;
        private readonly TaskCompletionSource<bool> _taskCompletion;
        private readonly IServiceBusClientAdmin _serviceBusClientAdmin;
        private readonly IServiceBusClientReceiver _serviceBusClientReceiver;
        private readonly FinishConsumerMiddlewareObservable _finishConsumerMiddlewareObservable;

        internal ReceiverListener(IServiceBusClientWrapper serviceBusClientWrapper, SubscriberContext subscriberContext)
        {
            _subscriberContext = subscriberContext ?? throw new ArgumentNullException(nameof(subscriberContext));

            _serviceBusClientAdmin = serviceBusClientWrapper.Admin;
            _serviceBusClientReceiver = serviceBusClientWrapper.Receiver;

            IsRunning = Task.FromResult(true);
            _taskCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

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

            _messagesBuffer = Channel.CreateBounded<IServiceBusMessageContext>(channelOptions);

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

        public IConnectHandle ConnectReceiveObserver(IReceiveObserver observer) => _receiveObservable.Connect(observer);

        public IConnectHandle ConnectFinishConsumerMiddlewareObserver(IFinishConsumerMiddlewareObserver observer) =>
            _finishConsumerMiddlewareObservable.Connect(observer);

        public Task CreateEntitiesIfNotExistAsync(SubscriberContext subscriberContext, CancellationToken stoppingToken)
            => _serviceBusClientAdmin.CreateEntitiesIfNotExistAsync(subscriberContext, stoppingToken);

        public async Task<bool> StartAsync(CancellationTokenSource cancellationTokenSource)
        {
            await CreateReceiversAsync(cancellationTokenSource);

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                        break;

                    await foreach (var receivedMessages in _serviceBusClientReceiver.StartConsumerAsync(
                                       cancellationToken: cancellationTokenSource.Token))
                    {
                        if (receivedMessages == null) continue;
                        await EnqueueAsync(receivedMessages, cancellationTokenSource.Token);
                    }
                }
                catch (OperationCanceledException e) when (e.CancellationToken == _cancellationToken)
                {
                    await StopAsync(cancellationTokenSource);
                }
                catch (Exception e)
                {
                    await StopAsync(cancellationTokenSource);
                }
            }

            return IsRunning.Result;
        }

        public async Task<bool> StopAsync(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                _messagesBuffer.Writer.TryComplete();

                await _readerTask;
                await _taskCompletion.Task;
                await _serviceBusClientReceiver.DisposeAsync();
                _readerTask.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                IsRunning = Task.FromResult(false);
            }

            return IsRunning.Result;
        }

        private async Task CreateReceiversAsync(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                await _receiveObservable.PreStartReceive(_subscriberContext);

                _serviceBusClientReceiver.TryCreateReceiver(_subscriberContext);

                await _receiveObservable.PostStartReceive(_subscriberContext);
            }
            catch (Exception e)
            {
                await _receiveObservable.FaultStartReceive(_subscriberContext, e);
                await StopAsync(cancellationTokenSource);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ValueTask EnqueueAsync(IServiceBusMessageContext receivedMessage, CancellationToken cancellationToken)
        {
            return _messagesBuffer.Writer.WriteAsync(receivedMessage, cancellationToken);
        }

        private async Task ReadFromChannel()
        {
            var batchCapacity = _subscriberContext.Specification.Consumer.BufferSize;

            try
            {
                while (await _messagesBuffer.Reader.WaitToReadAsync())
                {
                    var counter = 0;

                    var messageConsumerContext =
                        new MessageConsumerContext(_subscriberContext, _serviceBusClientReceiver, _cancellationToken);

                    while (counter < batchCapacity && _messagesBuffer.Reader.TryRead(out var receivedMessage))
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
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _taskCompletion.TrySetResult(false);
            }
        }
    }
}