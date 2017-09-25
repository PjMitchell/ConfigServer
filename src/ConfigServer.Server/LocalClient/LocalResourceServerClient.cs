using ConfigServer.Core;
using System;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class LocalResourceServerClient : IResourceServer
    {
        private readonly IResourceStore resourceStore;
        private readonly IClientIdProvider clientIdProvider;
        private readonly Uri pathToConfigServer;
        private readonly IConfigurationClientService configurationClientService;
        private readonly IConfigurationModelRegistry registry;
        public LocalResourceServerClient(IConfigProvider configProvider, IConfigurationClientService configurationClientService, IConfigurationModelRegistry registry, IResourceStore resourceStore,IClientIdProvider clientIdProvider, LocalServerClientOptions options)
        {
            this.resourceStore = resourceStore;
            this.clientIdProvider = clientIdProvider;
            this.pathToConfigServer = options.ConfigServerUri;
            this.configurationClientService = configurationClientService;
            this.registry = registry;
        }

        public Task<ResourceEntry> GetResourceAsync(string name)
        {
            return GetResourceAsync(name, clientIdProvider.GetCurrentClientId());
        }

        public async Task<ResourceEntry> GetResourceAsync(string name, string clientId)
        {
            var client = await configurationClientService.GetClientOrDefault(clientIdProvider.GetCurrentClientId());
            return await resourceStore.GetResource(name, new ConfigurationIdentity(client, registry.GetVersion())).ConfigureAwait(false);
        }

        public Uri GetResourceUri(string name)
        {
            return GetResourceUri(name, clientIdProvider.GetCurrentClientId());
        }

        public Uri GetResourceUri(string name, string clientId)
        {
            return new Uri(pathToConfigServer, $"Resource/{clientId}/{name}");
        }
    }
}
