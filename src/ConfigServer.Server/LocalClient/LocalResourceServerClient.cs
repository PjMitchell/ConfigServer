using ConfigServer.Core;
using System;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class LocalResourceServerClient : IResourceServer
    {
        private readonly string applicationId;
        private readonly IResourceStore resourceStore;
        private readonly Uri pathToConfigServer;
        private readonly IConfigurationClientService configurationClientService;
        private readonly IConfigurationModelRegistry registry;
        public LocalResourceServerClient(IConfigProvider configProvider, IConfigurationClientService configurationClientService, IConfigurationModelRegistry registry, IResourceStore resourceStore, LocalServerClientOptions options)
        {
            this.applicationId = options.ApplicationId;
            this.resourceStore = resourceStore;
            this.pathToConfigServer = options.ConfigServerUri;
            this.configurationClientService = configurationClientService;
            this.registry = registry;
        }

        public async Task<ResourceEntry> GetResourceAsync(string name)
        {
            var client = await configurationClientService.GetClientOrDefault(applicationId);
            return await resourceStore.GetResource(name, new ConfigurationIdentity(client, registry.GetVersion())).ConfigureAwait(false);
        }

        public Uri GetResourceUri(string name)
        {
            return new Uri(pathToConfigServer, $"Resource/{applicationId}/{name}");
        }
    }
}
