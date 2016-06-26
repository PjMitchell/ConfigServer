using System;
using System.Net;

namespace ConfigServer.Core.Client
{
    public class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException(Type type) : base($"Could not find configuration of type {type.Name}") { }
    }

    public class ConfigServerCommunicationException : Exception
    {
        public ConfigServerCommunicationException(Uri uri, HttpStatusCode returnedStatusCode) : base($"Failed to call {uri} with status code {returnedStatusCode}") { }
    }
}
