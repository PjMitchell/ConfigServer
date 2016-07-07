using System.Net.Http;

namespace ConfigServer.Core
{
    public interface IConfigServerClientAuthenticator
    {
        HttpClient ApplyAuthentication(HttpClient client);
    }


}
