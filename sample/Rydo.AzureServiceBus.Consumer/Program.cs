using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Rydo.AzureServiceBus.Client.Extensions;
using Rydo.AzureServiceBus.Consumer;

var builder = WebApplication.CreateBuilder(args);

var sbConnectionString = builder.Configuration.GetConnectionString("ServiceBus");

builder.Services.AddAzureServiceBusClient(config =>
{
    config.Producers.Configure(producers =>
    {
        producers.AddProducers(TopicNameConstants.AccountCreatedTopic);
        producers.AddProducers(TopicNameConstants.AccountUpdatedTopic);
    });

    config.Receiver.Configure(typeof(Program), queue =>
    {
        queue.Subscriber.AddSubscriber(TopicNameConstants.AccountCreatedTopic, configurator =>
        {
            configurator.BufferSize(1_000);
            configurator.MaxMessages(1_000);
        });
    });
    
    config.Receiver.Configure(typeof(Program), queue =>
    {
        queue.Subscriber.AddSubscriber(TopicNameConstants.AccountUpdatedTopic);
    });
});

builder.Services.AddAzureClients(config => { config.AddServiceBusClient(sbConnectionString); });
builder.Services.AddSingleton(sp => new ServiceBusAdministrationClient(sbConnectionString));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

await app.RunAsync();