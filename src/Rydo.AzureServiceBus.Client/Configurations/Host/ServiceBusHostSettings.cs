namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;

    internal sealed class ServiceBusHostSettings : IServiceBusHostSettings
    {
        internal ServiceBusHostSettings(ServiceBusClient serviceBusClient, ServiceBusAdministrationClient adminClient)
        {
            AdminClient = adminClient;
            ServiceBusClient = serviceBusClient;
        }

        public ServiceBusClient ServiceBusClient { get; }
        public ServiceBusAdministrationClient AdminClient { get; }
    }
}