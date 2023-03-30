namespace Rydo.AzureServiceBus.Client.Middlewares
{
    using System.Collections.Generic;

    public interface IMiddlewareConfigurationContainer
    {
        IList<MiddlewareConfiguration> Configs { get; }
        MiddlewareConfiguration FinallyProcess { get; }
        int TotalConfigurations { get; }
    }
}