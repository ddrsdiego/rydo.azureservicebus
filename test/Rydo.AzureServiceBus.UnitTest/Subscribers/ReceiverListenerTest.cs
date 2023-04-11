namespace Rydo.AzureServiceBus.UnitTest.Subscribers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Core;
    using Client.Consumers.Subscribers;
    using Client.Extensions;
    using Consumer.ConsumerHandlers;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    public class ReceiverListenerTest
    {
        private const string ConnectionString =
            "Endpoint=sb://rydo-azure-servicebus-test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YfN2b8jT84759bZy5sMhd0P+3K/qHqO81I5VrNrJYkI=";

        [Test]
        public async Task Should_Is_Running_Listener_When_Add_To_ListenerContainer_1()
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddAzureServiceBusClient(configurator =>
            {
                configurator.Host.Configure(ConnectionString);
                configurator.Receiver.Configure<AccountUpdatedConsumerHandler>();
            });

            var provider = services.BuildServiceProvider();

            var receiverListenerContainer = provider.GetRequiredService<IReceiverListenerContainer>();
            var subscriberContextContainer = provider.GetRequiredService<ISubscriberContextContainer>();

            subscriberContextContainer.Contexts.Count.Should().Be(1);
            receiverListenerContainer.Listeners.Count.Should().Be(1);

            var receiverListener = receiverListenerContainer.Listeners.First().Value;
            receiverListener.IsRunning.Result.Should().BeTrue();

            var token = new CancellationTokenSource();

            token.CancelAfter(TimeSpan.FromMilliseconds(3_000));
            var state = await receiverListener.StartAsync(token).ConfigureAwait(false);

            state.Should().BeFalse();
        }

        [Test]
        public void Should_Is_Running_Listener_When_Add_To_ListenerContainer()
        {
            var services = new ServiceCollection();

            var hostAddress = AzureServiceBusEndpointUriCreator.Create("rydoazureservicebus-build");

            services.AddLogging();
            services.AddAzureServiceBusClient(configurator =>
            {
                configurator.Host.Configure(ConnectionString);
                configurator.Receiver.Configure<AccountUpdatedConsumerHandler>();
            });

            var provider = services.BuildServiceProvider();

            var receiverListenerContainer = provider.GetRequiredService<IReceiverListenerContainer>();
            var subscriberContextContainer = provider.GetRequiredService<ISubscriberContextContainer>();

            subscriberContextContainer.Contexts.Count.Should().Be(1);
            receiverListenerContainer.Listeners.Count.Should().Be(1);

            var receiverListener = receiverListenerContainer.Listeners.First().Value;
            receiverListener.IsRunning.Result.Should().BeTrue();
        }
    }

    public static class AzureServiceBusEndpointUriCreator
    {
        public static Uri Create(string serviceBusNamespace, string entityPath = null,
            string azureEndPoint = "servicebus.windows.net")
        {
            var endpoint = $"sb://{serviceBusNamespace}.{azureEndPoint}/{entityPath}";

            return new Uri(endpoint);
        }
    }

    public interface IServiceBusTokenProviderSettings
    {
        AzureNamedKeyCredential NamedKeyCredential { get; }
        AzureSasCredential SasCredential { get; }
        TokenCredential TokenCredential { get; }
    }

    public class TestAzureServiceBusAccountSettings :
        IServiceBusTokenProviderSettings
    {
        public TestAzureServiceBusAccountSettings()
        {
        }

        public AzureNamedKeyCredential NamedKeyCredential =>
            new AzureNamedKeyCredential("MassTransitBuild", "YfN2b8jT84759bZy5sMhd0P+3K/qHqO81I5VrNrJYkI=");

        public AzureSasCredential SasCredential { get; }
        public TokenCredential TokenCredential { get; }
    }
}