using System;
using System.Net;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents errors that occur when a failure to communicate with ConfigServer
    /// </summary>
    public class ConfigServerCommunicationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ConfigServerCommunicationException class for a given request uri and returned status code/
        /// </summary>
        /// <param name="uri">Requested Uri that failed</param>
        /// <param name="returnedStatusCode">Returned http staus code</param>
        public ConfigServerCommunicationException(Uri uri, HttpStatusCode returnedStatusCode) : base($"Failed to call {uri} with status code {returnedStatusCode}") { }
    }
}
