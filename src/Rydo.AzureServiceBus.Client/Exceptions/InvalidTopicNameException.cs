namespace Rydo.AzureServiceBus.Client.Exceptions
{
    using System;

    public class InvalidTopicNameException : Exception
    {
        public InvalidTopicNameException(string modelName)
            : base(CreateMessage(modelName))
        {
        }

        private static string CreateMessage(string modelName) =>
            $"The {modelName} object was marked with the TopicProducerAttribute attribute, but no topic name was included in its constructor.";
    }
}