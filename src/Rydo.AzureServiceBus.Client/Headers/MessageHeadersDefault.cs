namespace Rydo.AzureServiceBus.Client.Headers
{
    using System.Net.Mime;

    internal static class MessageHeadersDefault
    {
        public const string ProducedAt = "produced_at";
        public const string CorrelationId = "cid";
        public const string Env = "env";
        public const string Producer = "producer";
        public const string Signature = "signature";
        public const string Session = "session";
        public const string ContentType = "content-type";
        public const string ConsumerRole = "consumer-role";
        public const string Meta = "meta";
        public const string DefaultContentTypeValue = MediaTypeNames.Application.Json;
        public const string BrokerAccountId = "broker-account-id";
        public const string SkipSerialization = "skip_serialization";
        public const string MessageToRetry = "message_to_retry";
        public const string PartitionKey = "partition_key";
        public const string Priority = "priority";
        public const string DeadLetterReplayAttempts = "deadletter-replay-attempts";
        public const string DeadLetterReplayInterval = "deadletter-replay-interval";
    }
}