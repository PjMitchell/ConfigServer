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
        readonly IConfigServerClientAuthenticator authenticator;

        /// <summary>
        /// Contructor for IHttpClientWrapper
        /// </summary>
        /// <param name="authenticator">Authenticator for wrapper</param>
        public HttpClientWrapper(IConfigServerClientAuthenticator authenticator)
        {
            this.authenticator = authenticator;
        }

        /// <summary>
        /// Gets Uri
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <returns>HttpResponseMessage</returns>
        public HttpResponseMessage Get(Uri uri)
        {
            return GetAsync(uri).Result;
        }

        /// <summary>
        /// Gets Uri Async
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <returns>HttpResponseMessage</returns>
        public async Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            using (var httpClient = new HttpClient())
            {              
                return await authenticator.ApplyAuthentication(httpClient).GetAsync(uri);                
            }
        }
    }
}
