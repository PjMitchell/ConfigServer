using System.Net.Http;

namespace ConfigServer.Core.Client
{
    public interface IConfigServerClientAuthenticator
    {
        HttpClient ApplyAuthentication(HttpClient client);
    }

    public class DefaultConfigServerClientAuthenticator : IConfigServerClientAuthenticator
    {
        public HttpClient ApplyAuthentication(HttpClient client)
        {
            return client;
        }
    }
}
