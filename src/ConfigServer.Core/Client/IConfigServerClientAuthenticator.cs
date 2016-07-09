using System.Net.Http;

namespace ConfigServer.Core
{
    /// <summary>
    /// Applies authentication headers to http client request
    /// </summary>
    public interface IConfigServerClientAuthenticator
    {
        /// <summary>
        /// Applies authentication headers to http client request
        /// </summary>
        /// <param name="client">httpclient used in ConfigServer request</param>
        /// <returns>httpclient used in ConfigServer request</returns>
        HttpClient ApplyAuthentication(HttpClient client);
    }
}
