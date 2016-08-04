using System.Net.Http;

namespace ConfigServer.Client
{
    internal class DefaultConfigServerClientAuthenticator : IConfigServerClientAuthenticator
    {
        public HttpClient ApplyAuthentication(HttpClient client)
        {
            return client;
        }
    }
}
