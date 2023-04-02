namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;

    internal sealed class ServiceBusHostSettings : IServiceBusHostSettings
    {
        public ServiceBusHostSettings(string connectionString, ServiceBusClient serviceBusClient,
            ServiceBusAdministrationClient serviceBusAdministrationClient)
        {
            ConnectionString = connectionString;
            ServiceBusClient = serviceBusClient;
            ServiceBusAdministrationClient = serviceBusAdministrationClient;
        }

        public string ConnectionString { get; }
        public ServiceBusClient ServiceBusClient { get; }
        public ServiceBusAdministrationClient ServiceBusAdministrationClient { get; }
    }
}