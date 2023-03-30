namespace Rydo.AzureServiceBus.Client.Producers
{
    using System;

    public readonly struct ProducerResponse
    {
        public ProducerResponse(ProducerRequest request)
        {
            Request = request;
            ResponseAt = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Date and time of the response provided by the broker
        /// </summary>
        public readonly DateTime ResponseAt;
        
        /// <summary>
        /// Original request associated with response
        /// </summary>
        public readonly ProducerRequest Request;
    }
}