using System;

namespace ConfigServer.Server
{
    internal class LocalServerClientOptions
    {
        public LocalServerClientOptions(Uri configServeruri)
        {
            ConfigServerUri = configServeruri;
        }

        public Uri ConfigServerUri { get; }
    }
}
