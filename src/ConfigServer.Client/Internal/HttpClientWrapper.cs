using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConfigServer.Client
{
    /// <summary>
    /// Wrapper for httpClient 
    /// </summary>
    public interface IHttpClientWrapper
    {
        /// <summary>
        /// Gets http response for uri
        /// </summary>
        /// <param name="uri">uri to be requested</param>
        /// <returns>HttpResponseMessage from request</returns>
        Task<HttpResponseMessage> GetAsync(Uri uri);
    }

    /// <summary>
    /// Wrapper for HttpClient
    /// </summary>
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly IConfigServerClientAuthenticator authenticator;
        private readonly HttpClient client;

        /// <summary>
        /// Contructor for IHttpClientWrapper
        /// </summary>
        /// <param name="authenticator">Authenticator for wrapper</param>
        public HttpClientWrapper(IConfigServerClientAuthenticator authenticator)
        {
            this.authenticator = authenticator;
            client = new HttpClient();
        }

        /// <summary>
        /// Gets Uri Async
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> GetAsync(Uri uri)
        {      
            return await authenticator.ApplyAuthentication(client).GetAsync(uri);  
        }
    }
}
