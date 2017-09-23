using ConfigServer.Core;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
namespace ConfigServer.Client
{
    internal class ResourceServerClient : IResourceServer
    {
        private readonly ConfigServerClientOptions options;
        private readonly IHttpClientWrapper client;

        public ResourceServerClient(IHttpClientWrapper client, ConfigServerClientOptions options)
        {
            this.client = client;
            this.options = options;
        }

        /// <summary>
        /// Gets resource file
        /// </summary>
        /// <param name="name">Name of resource</param>
        /// <returns>Resource response</returns>
        public async Task<ResourceEntry> GetResourceAsync(string name)
        {
            var uri = GetResourceUri(name);
            var response = await client.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new ResourceEntry { Name = name };

            if (!response.IsSuccessStatusCode)
                throw new ConfigServerCommunicationException(uri, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            return new ResourceEntry { Name = name, HasEntry = true, Content = await response.Content.ReadAsStreamAsync() }; ;
        }
        
        /// <summary>
        /// Get resource uri
        /// </summary>
        /// <param name="name">Name of resource</param>
        /// <returns>Resource uri</returns>
        public Uri GetResourceUri(string name)
        {
            return new Uri($"{options.ConfigServer}/Resource/{options.ClientId}/{name}");
        }
    }
}
