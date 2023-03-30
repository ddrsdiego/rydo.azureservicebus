namespace Rydo.AzureServiceBus.UnitTest.Producers
{
    using System;
    using Client.Producers;
    using Client.Serialization;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    public class ProducerContextContainerTest
    {
        [Test]
        public void Should_Failed_Add_Topic_In_Container_When_Topic_Is_Duplicated()
        {
            const string topicName = "rydo-azureservicebus-account-created";
            var services = new ServiceCollection();

            var sut = new ProducerContextContainer(services);
            sut.TryAdd(topicName);

            Assert.Throws<InvalidOperationException>(() => sut.TryAdd(topicName));
        }

        [Test]
        public void Should_Failed_Add_Topic_In_Container_When_Topic_Is_Empty()
        {
            var services = new ServiceCollection();

            var sut = new ProducerContextContainer(services);
            Assert.Throws<ArgumentNullException>(() => sut.TryAdd(string.Empty));
        }

        [Test]
        public void Should_Failed_Add_Topic_In_Container_When_Topic_Is_Null()
        {
            var services = new ServiceCollection();

            var sut = new ProducerContextContainer(services);

            Assert.Throws<ArgumentNullException>(() => sut.TryAdd(null));
        }

        [Test]
        public void Should_Add_Topic_In_Container()
        {
            var services = new ServiceCollection();

            services.AddLogging();

            var sut = new ProducerContextContainer(services);
            sut.TryAdd("rydo-azureservicebus-account-created");

            var serviceProvider = services.BuildServiceProvider();

            var serializer = serviceProvider.GetRequiredService<ISerializer>();
            sut.Entries.Count.Should().Be(1);
        }
    }
}