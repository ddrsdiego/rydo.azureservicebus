// namespace Rydo.AzureServiceBus.Producer.Endpoints.v1
// {
//     using Ardalis.ApiEndpoints;
//     using Azure.Messaging.ServiceBus;
//     using Microsoft.AspNetCore.Mvc;
//
//     public sealed class PostCreateAccountEndpoint : EndpointBaseAsync
//         .WithoutRequest
//         .WithActionResult
//     {
//         private readonly ServiceBusClient _serviceBusClient;
//
//         public PostCreateAccountEndpoint(ServiceBusClient serviceBusClient)
//         {
//             _serviceBusClient = serviceBusClient;
//         }
//
//         [HttpPost("api/v1/accounts")]
//         public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = new())
//         {
//             const string messageBody = "This is a sample message.";
//             
//             var sender = _serviceBusClient.CreateSender("rydo-azureservicebus-account-created");
//
//             var message = new ServiceBusMessage(messageBody);
//             await sender.SendMessageAsync(message, cancellationToken);
//
//             return Accepted();
//         }
//     }
// }