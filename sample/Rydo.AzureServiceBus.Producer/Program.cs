using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.OpenApi.Models;
using Rydo.AzureServiceBus.Consumer.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureClients(config =>
{
    var connectionString = builder.Configuration.GetConnectionString("ServiceBus");

    config.AddServiceBusClient(connectionString);
    config.AddServiceBusAdministrationClient(connectionString);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo {Title = Assembly.GetEntryAssembly()?.GetName().Name}));

var app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", Assembly.GetEntryAssembly()?.GetName().Name));

app.MapPost("api/v1/accounts", async (ServiceBusClient serviceBusClient) =>
{
    const int capacity = 100;
    const string messageBody = "This is a sample message.";
    const string accountCreatedTopic = "azureservicebus-sample-account-created";

    var sender = serviceBusClient.CreateSender(accountCreatedTopic);
    
    var tasks = new List<Task>(capacity);
    for (var index = 0; index < capacity; index++)
    {
        var accountCreatedMessage = new AccountCreated("5090016");
        var payload = JsonSerializer.SerializeToUtf8Bytes(accountCreatedMessage);
        var message = new ServiceBusMessage(payload)
        {
            ContentType = MediaTypeNames.Application.Json,
            PartitionKey = accountCreatedMessage.AccountNumber
        };

        tasks.Add(sender.SendMessageAsync(message));
        await Task.WhenAll(tasks);

    }
    
    return Results.Accepted();
});

await app.RunAsync();