namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;

    public interface IServiceBusHostSettings
    {
        string ConnectionString { get; }

        /// <summary>
        /// A custom client that will be used instead of one defined by the settings provided here.
        /// </summary>
        ServiceBusClient ServiceBusClient { get; }

        /// <summary>
        /// A custom administration client that will be used instead of one defined by the settings provided here.
        /// </summary>
        ServiceBusAdministrationClient AdminClient { get; }
    }
}