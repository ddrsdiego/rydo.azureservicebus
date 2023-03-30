using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Rydo.AzureServiceBus.Client.Extensions;

var builder = WebApplication.CreateBuilder(args);

var sbConnectionString = builder.Configuration.GetConnectionString("ServiceBus");

const string accountUpdatedTopic = "azureservicebus-sample-account-updated";
const string accountCreatedTopic = "azureservicebus-sample-account-created";

builder.Services.AddAzureServiceBusClient(config =>
{
    config.Producer.Configure(producers =>
    {
        producers.AddProducers(accountCreatedTopic);
        producers.AddProducers(accountUpdatedTopic);
    });

    config.Consumer.Configure(typeof(Program), consumers =>
    {
        consumers.AddSubscriber(accountCreatedTopic, configurator =>
        {
            configurator.BufferSize(1_000);
            configurator.MaxMessages(1_000);
        });
        consumers.AddSubscriber(accountUpdatedTopic);
    });
});

builder.Services.AddAzureClients(config => { config.AddServiceBusClient(sbConnectionString); });
builder.Services.AddSingleton(sp => new ServiceBusAdministrationClient(sbConnectionString));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

await app.RunAsync();