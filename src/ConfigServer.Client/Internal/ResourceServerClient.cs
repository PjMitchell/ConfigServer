using ConfigServer.Core;
using System;
using System.Threading.Tasks;
namespace ConfigServer.Client
{
    internal class ResourceServerClient : IResourceServer
    {
        private readonly ConfigServerClientOptions options;
        private readonly IClientIdProvider clientIdProvider;
        private readonly IHttpClientWrapper client;

        public ResourceServerClient(IHttpClientWrapper client, ConfigServerClientOptions options, IClientIdProvider clientIdProvider)
        {
            this.client = client;
            this.options = options;
            this.clientIdProvider = clientIdProvider;
        }

        public Task<ResourceEntry> GetResourceAsync(string name)
        {
            return GetResourceAsync(name, clientIdProvider.GetCurrentClientId());
        }

        public async Task<ResourceEntry> GetResourceAsync(string name, string clientId)
        {
            var uri = GetResourceUri(name, clientId);
            var response = await client.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new ResourceEntry { Name = name };

            if (!response.IsSuccessStatusCode)
                throw new ConfigServerCommunicationException(uri, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            return new ResourceEntry { Name = name, HasEntry = true, Content = await response.Content.ReadAsStreamAsync() }; ;
        }

        public Uri GetResourceUri(string name)
        {
            return GetResourceUri(name, clientIdProvider.GetCurrentClientId());
        }

        public Uri GetResourceUri(string name, string clientId)
        {
            return new Uri($"{options.ConfigServer}/Resource/{clientId}/{name}");
        }
    }
}
