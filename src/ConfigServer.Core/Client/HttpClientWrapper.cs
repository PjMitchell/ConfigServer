using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConfigServer.Core.Client
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(Uri uri);
    }

    public class HttpClientWrapper : IHttpClientWrapper
    {
        public HttpResponseMessage Get(Uri uri)
        {
            return GetAsync(uri).Result;
        }

        public async Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetAsync(uri);                
            }
        }
    }
}
