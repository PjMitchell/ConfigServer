using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace ConfigServer.Core.Internal
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

    internal class HttpClientWrapper : IHttpClientWrapper
    {
        readonly IConfigServerClientAuthenticator authenticator;

        public HttpClientWrapper(IConfigServerClientAuthenticator authenticator)
        {
            this.authenticator = authenticator;
        }

        public HttpResponseMessage Get(Uri uri)
        {
            return GetAsync(uri).Result;
        }

        public async Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            using (var httpClient = new HttpClient())
            {              
                return await authenticator.ApplyAuthentication(httpClient).GetAsync(uri);                
            }
        }
    }
}
