using ConfigServer.Core;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class LocalConfigServerClient : IConfigServer
    {
        private readonly IConfigProvider configProvider;
        private readonly string applicationId;
        private readonly IResourceStore resourceStore;
        private readonly Uri pathToConfigServer;
        private readonly IConfigurationClientService configurationClientService;
        public LocalConfigServerClient(IConfigProvider configProvider,IConfigurationClientService configurationClientService, IResourceStore resourceStore, string applicationId, Uri pathToConfigServer)
        {
            this.configProvider = configProvider;
            this.applicationId = applicationId;
            this.resourceStore = resourceStore;
            this.pathToConfigServer = pathToConfigServer;
            this.configurationClientService = configurationClientService;
        }

        public async Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>() where TConfig : class, new()
        {
            var client = await configurationClientService.GetClientOrDefault(applicationId);
            var config = await configProvider.GetCollectionAsync<TConfig>(new ConfigurationIdentity(client)).ConfigureAwait(false);
            return config;
        }

        public async Task<TConfig> GetConfigAsync<TConfig>() where TConfig : class, new()
        {
            var client = await configurationClientService.GetClientOrDefault(applicationId);
            var config = await configProvider.GetAsync<TConfig>(new ConfigurationIdentity(client)).ConfigureAwait(false);
            return config.Configuration;
        }

        public async Task<object> GetConfigAsync(Type type)
        {
            var client = await configurationClientService.GetClientOrDefault(applicationId);
            var config = await configProvider.GetAsync(type, new ConfigurationIdentity(client)).ConfigureAwait(false);
            return config.GetConfiguration();
        }

        public IEnumerable<TConfig> GetCollectionConfig<TConfig>() where TConfig : class, new()
        {
            return GetCollectionConfigAsync<TConfig>().Result;
        }

        public TConfig GetConfig<TConfig>() where TConfig : class, new()
        {
            
            return GetConfigAsync<TConfig>().Result;
        }

        public object GetConfig(Type type)
        {
            return GetConfigAsync(type).Result;
        }

        public async Task<ResourceEntry> GetResourceAsync(string name)
        {
            var client = await configurationClientService.GetClientOrDefault(applicationId);
            return await resourceStore.GetResource(name, new ConfigurationIdentity(client)).ConfigureAwait(false);
        }

        public ResourceEntry GetResource(string name)
        {
            return GetResourceAsync(name).Result;
        }

        public Uri GetResourceUri(string name)
        {
            return new Uri(pathToConfigServer, $"Resource/{applicationId}/{name}");
        }
    }
}
