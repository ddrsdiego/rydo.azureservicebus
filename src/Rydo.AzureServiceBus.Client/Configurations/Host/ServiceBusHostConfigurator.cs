namespace Rydo.AzureServiceBus.Client.Configurations.Host
{
    using System;
    using Azure.Messaging.ServiceBus.Administration;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public sealed class ServiceBusHostConfigurator
    {
        private readonly IServiceCollection _services;

        internal ServiceBusHostConfigurator(IServiceCollection services)
        {
            _services = services;
        }

        public void Configure(string connectionString)
        {
            // var properties = ServiceBusConnectionStringProperties.Parse(connectionString);

            _services.AddAzureClients(config => config.AddServiceBusClient(connectionString));
            _services.TryAddSingleton(sp => new ServiceBusAdministrationClient(connectionString));
        }
    }
    
    internal static class AzureServiceBusEndpointUriCreator
    {
        public static Uri Create(string serviceBusNamespace, string entityPath = null,
            string azureEndPoint = "servicebus.windows.net")
        {
            var endpoint = $"sb://{serviceBusNamespace}.{azureEndPoint}/{entityPath}";

            return new Uri(endpoint);
        }
    }
}