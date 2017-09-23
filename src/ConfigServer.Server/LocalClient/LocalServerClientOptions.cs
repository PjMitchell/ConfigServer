using System;

namespace ConfigServer.Server
{
    internal class LocalServerClientOptions
    {
        public LocalServerClientOptions(string applicationId, Uri configServeruri)
        {
            ApplicationId = applicationId;
            ConfigServerUri = configServeruri;
        }

        public string ApplicationId { get; }
        public Uri ConfigServerUri { get; }
    }
}
