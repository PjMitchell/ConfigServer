using System.Net.Http;

namespace ConfigServer.Core.Internal
{
    internal class DefaultConfigServerClientAuthenticator : IConfigServerClientAuthenticator
    {
        public HttpClient ApplyAuthentication(HttpClient client)
        {
            return client;
        }
    }
}
