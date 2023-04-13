using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.OpenApi.Models;
using Rydo.AzureServiceBus.Consumer.ConsumerHandlers;
using Rydo.AzureServiceBus.Producer;
using AccountCreated = Rydo.AzureServiceBus.Producer.ConsumerHandlers.AccountCreated;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ServiceBus");
builder.Services.AddAzureClients(config =>
{
    config.AddServiceBusClient(connectionString);
    config.AddServiceBusAdministrationClient(connectionString);
});

builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddHostedService<PublisherWorker>();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo {Title = Assembly.GetEntryAssembly()?.GetName().Name}));

// builder.Services.AddAzureServiceBusClient(x =>
// {
//     x.Host.Configure(connectionString);
//     x.Receiver.Configure<AccountCreatedConsumerHandler>();
// });

var app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", Assembly.GetEntryAssembly()?.GetName().Name));

app.MapPost("api/v1/accounts/{amount:int}", async (ServiceBusClient serviceBusClient, int amount) =>
{
    var sender = serviceBusClient.CreateSender(TopicNameConstants.AccountCreated);
    
    var tasks = Enumerable.Range(0, amount).Select(async index =>
    {
        var accountNumber = index.ToString("0000000");
        var accountCreatedMessage = new AccountCreated(accountNumber);

        var producerName = Assembly.GetExecutingAssembly().GetName().Name?.ToLowerInvariant();
        var payload = JsonSerializer.SerializeToUtf8Bytes(accountCreatedMessage);

        var message = new ServiceBusMessage(payload)
        {
            ContentType = MediaTypeNames.Application.Json,
            ApplicationProperties =
            {
                new KeyValuePair<string, object>("producer", producerName),
            },
            PartitionKey = accountCreatedMessage.AccountNumber
        };
        
        await sender.SendMessageAsync(message);
    });
        
    await Task.WhenAll(tasks);
    await Task.Delay(1_000);
    
    return Results.Accepted();
});

app.MapPost("api/v1/accounts/banlance", async (ServiceBusClient serviceBusClient) =>
{
    const int capacity = 1_000;
    const string accountUpdatesTopic = "rydo.customers.account-account-updated";

    var sender = serviceBusClient.CreateSender(TopicNameConstants.AccountUpdated);

    var tasks = new List<Task>(capacity);
    for (var index = 1; index <= capacity; index++)
    {
        var accountNumber = index.ToString("0000000");

        var accountCreatedMessage = new AccountUpdated(accountNumber);

        var producerName = Assembly.GetExecutingAssembly().GetName().Name;
        var payload = JsonSerializer.SerializeToUtf8Bytes(accountCreatedMessage);

        var message = new ServiceBusMessage(payload)
        {
            ContentType = MediaTypeNames.Application.Json,
            ApplicationProperties =
            {
                new KeyValuePair<string, object>("producer", producerName),
            },
            PartitionKey = accountCreatedMessage.AccountNumber
        };

        tasks.Add(sender.SendMessageAsync(message));
    }

    await Task.WhenAll(tasks);
    return Results.Accepted();
});

await app.RunAsync();