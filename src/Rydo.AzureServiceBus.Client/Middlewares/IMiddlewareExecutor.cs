namespace Rydo.AzureServiceBus.Client.Middlewares
{
    using System;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.Extensions.DependencyInjection;

    public interface IMiddlewareExecutor
    {
        Task Execute(IServiceScope scope, MessageConsumerContext context,
            Func<MessageConsumerContext, Task> nextOperation);
    }
}