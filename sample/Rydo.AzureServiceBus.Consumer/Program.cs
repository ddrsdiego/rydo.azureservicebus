using System.Reflection;
using Rydo.AzureServiceBus.Client.Extensions;
using Rydo.AzureServiceBus.Consumer;
using Rydo.AzureServiceBus.Consumer.ConsumerHandlers;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

var sbConnectionString = builder.Configuration.GetConnectionString("ServiceBus");

builder.Services.AddAzureServiceBusClient(config =>
{
    config.Host.Configure(sbConnectionString);
    config.Producers.Configure(producers =>
    {
        producers.AddProducers(TopicNameConstants.AccountCreated);
        producers.AddProducers(TopicNameConstants.AccountUpdated);
    });
    config.Receiver.Configure<AccountCreatedConsumerHandler>(receiver =>
    {
        receiver.Subscriber.Add(sub =>
        {
            sub.BufferSize(1_000);
            sub.MaxMessages(1_000);
        });
    });
    config.Receiver.Configure<AccountUpdatedConsumerHandler>();
});

builder.Host.UseSerilog((context, sp, config) =>
{
    var assembly = Assembly.GetExecutingAssembly().GetName();
    var isDebug = context.Configuration.GetSection("IsDebug").Get<bool>();

    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Destructure.ToMaximumCollectionCount(10)
        .Destructure.ToMaximumStringLength(1024)
        .Destructure.ToMaximumDepth(5)
        .Enrich.WithProperty("Jornada", "Foundations")
        .Enrich.WithProperty("Assembly", $"{assembly.Name}")
        .Enrich.WithProperty("Version", $"{assembly.Version}")
        .Enrich
        .WithExceptionDetails(new DestructuringOptionsBuilder()
            .WithDefaultDestructurers()
            .WithRootName("Exception"));

    if (isDebug)
    {
        config.WriteTo.Async(sinkConfigurations =>
            sinkConfigurations.Console(
                outputTemplate:
                "{Timestamp:HH:mm:ss} {Level:u3} => {Message:lj}{Properties:j}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Debug, theme: AnsiConsoleTheme.Code));
    }
    else
    {
        config.WriteTo.Async(sinkConfigurations =>
            sinkConfigurations.Console(new ElasticsearchJsonFormatter(inlineFields: true,
                renderMessageTemplate: false)));
    }
});
var app = builder.Build();


app.MapGet("/", () => "Hello World!");

await app.RunAsync();