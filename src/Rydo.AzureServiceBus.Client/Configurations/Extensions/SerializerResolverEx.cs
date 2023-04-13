namespace Rydo.AzureServiceBus.Client.Configurations.Extensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serialization;

    internal static class SerializerResolverEx
    {
        public static ISerializer TryResolveSerializer(this IServiceBusClientConfigurator configurator,
            IServiceProvider sp)
        {
            var logger = sp.GetRequiredService<ILogger<SystemTextJsonSerializer>>();
            return new SystemTextJsonSerializer(logger);
        }
    }
}