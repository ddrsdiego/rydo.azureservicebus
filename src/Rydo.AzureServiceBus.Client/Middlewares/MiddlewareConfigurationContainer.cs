namespace Rydo.AzureServiceBus.Client.Middlewares
{
    using System.Collections.Generic;

    public class MiddlewareConfigurationContainer : IMiddlewareConfigurationContainer
    {
        public MiddlewareConfigurationContainer(IList<MiddlewareConfiguration> configs,
            MiddlewareConfiguration finallyProcess)
        {
            Configs = configs;
            FinallyProcess = finallyProcess;
        }

        public IList<MiddlewareConfiguration> Configs { get; }
        public MiddlewareConfiguration FinallyProcess { get; }
        public int TotalConfigurations => Configs.Count + 1;
    }
}