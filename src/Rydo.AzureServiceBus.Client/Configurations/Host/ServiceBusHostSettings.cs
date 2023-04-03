namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;

    internal sealed class ServiceBusHostSettings : IServiceBusHostSettings
    {
        public ServiceBusHostSettings(string connectionString, ServiceBusClient serviceBusClient,
            ServiceBusAdministrationClient adminClient)
        {
            ConnectionString = connectionString;
            ServiceBusClient = serviceBusClient;
            AdminClient = adminClient;
        }

        public string ConnectionString { get; }
        public ServiceBusClient ServiceBusClient { get; }
        public ServiceBusAdministrationClient AdminClient { get; }
    }
}