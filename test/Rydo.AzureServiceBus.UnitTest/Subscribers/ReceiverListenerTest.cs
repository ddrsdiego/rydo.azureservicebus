namespace Rydo.AzureServiceBus.UnitTest.Subscribers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Client.Configurations;
    using Client.Consumers.Subscribers;
    using Consumer.ConsumerHandlers;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    public class ReceiverListenerTest
    {
        [Test]
        public async Task Should_Is_Running_Listener_When_Add_To_ListenerContainer_1()
        {
            var services = new ServiceCollection();
            var sut = new ServiceBusClientConfigurator(services);

            services.AddLogging();

            sut.Receiver.Configure<AccountUpdatedConsumerHandler>();

            var provider = services.BuildServiceProvider();
            
            var receiverListenerContainer = provider.GetRequiredService<IReceiverListenerContainer>();
            var subscriberContextContainer = provider.GetRequiredService<ISubscriberContextContainer>();

            subscriberContextContainer.Contexts.Count.Should().Be(1);
            receiverListenerContainer.Listeners.Count.Should().Be(1);
            
            var receiverListener = receiverListenerContainer.Listeners.First().Value;
            receiverListener.IsRunning.Result.Should().BeTrue();

            var token = new CancellationToken();
            
            // await receiverListener.StartAsync(token).ConfigureAwait(false);

            await Task.CompletedTask;
        }
        
        [Test]
        public void Should_Is_Running_Listener_When_Add_To_ListenerContainer()
        {
            var services = new ServiceCollection();
            var sut = new ServiceBusClientConfigurator(services);

            services.AddLogging();

            sut.Receiver.Configure<AccountUpdatedConsumerHandler>();

            var provider = services.BuildServiceProvider();
            
            var receiverListenerContainer = provider.GetRequiredService<IReceiverListenerContainer>();
            var subscriberContextContainer = provider.GetRequiredService<ISubscriberContextContainer>();

            subscriberContextContainer.Contexts.Count.Should().Be(1);
            receiverListenerContainer.Listeners.Count.Should().Be(1);
            
            var receiverListener = receiverListenerContainer.Listeners.First().Value;
            receiverListener.IsRunning.Result.Should().BeTrue();
        }
    }
}