using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Azure;
using Rydo.AzureServiceBus.Client.Extensions;

var builder = WebApplication.CreateBuilder(args);

var sbConnectionString = builder.Configuration.GetConnectionString("ServiceBus");

builder.Services.AddAzureServiceBusClient(config =>
{
    // config.Producer.Configure(producers =>
    // {
    //     producers.TryAdd("rydo-azureservicebus-account-created");
    //     producers.TryAdd("rydo-azureservicebus-account-updated");
    // });

    config.Consumer.Configure(typeof(Program), consumers =>
    {
        consumers.AddSubscriber("azureservicebus-sample-account-created");
        consumers.AddSubscriber("azureservicebus-sample-account-updated");
    });
});

builder.Services.AddAzureClients(config => { config.AddServiceBusClient(sbConnectionString); });
builder.Services.AddSingleton(sp => new ServiceBusAdministrationClient(sbConnectionString));

// builder.Services.AddHostedService<AccountCreatedSubscriptionWorker>();
// builder.Services.AddHostedService<AccountUpdatedSubscriptionWorker>();

var app = builder.Build();

// var a = app.Services.GetRequiredService<IProducerContextContainer>();

app.MapGet("/", () => "Hello World!");

await app.RunAsync();