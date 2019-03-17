using ConfigServer.Core;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class LocalConfigServerClient : IConfigServer
    {
        private readonly IConfigProvider configProvider;
        private readonly IConfigurationClientService configurationClientService;
        private readonly IConfigurationModelRegistry registry;
        private readonly IClientIdProvider clientIdProvider;

        public LocalConfigServerClient(IConfigProvider configProvider,IConfigurationClientService configurationClientService, IConfigurationModelRegistry registry, IClientIdProvider clientIdProvider)
        {
            this.configProvider = configProvider;
            this.configurationClientService = configurationClientService;
            this.registry = registry;
            this.clientIdProvider = clientIdProvider;
        }

        public Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>() where TConfig : class, new()
        {
            return GetCollectionConfigAsync<TConfig>(clientIdProvider.GetCurrentClientId());
        }

        public async Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>(string clientId) where TConfig : class, new()
        {
            var client = await configurationClientService.GetClientOrDefault(clientId);
            var config = await configProvider.GetCollectionAsync<TConfig>(new ConfigurationIdentity(client, registry.GetVersion())).ConfigureAwait(false);
            return config;
        }

        public Task<TConfig> GetConfigAsync<TConfig>() where TConfig : class, new()
        {
            return GetConfigAsync<TConfig>(clientIdProvider.GetCurrentClientId());
        }

        public async Task<TConfig> GetConfigAsync<TConfig>(string clientId) where TConfig : class, new()
        {
            var client = await configurationClientService.GetClientOrDefault(clientId);
            if (client == null)
                throw new ConfigurationClientNotFoundException(clientId);
            var config = await configProvider.GetAsync<TConfig>(new ConfigurationIdentity(client, registry.GetVersion())).ConfigureAwait(false);
            return config.Configuration;
        }

        public Task<object> GetConfigAsync(Type type)
        {
            return GetConfigAsync(type, clientIdProvider.GetCurrentClientId());
        }
        
        public async Task<object> GetConfigAsync(Type type, string clientId)
        {
            var client = await configurationClientService.GetClientOrDefault(clientIdProvider.GetCurrentClientId());
            var config = await configProvider.GetAsync(type, new ConfigurationIdentity(client, registry.GetVersion())).ConfigureAwait(false);
            return config.GetConfiguration();
        }
    }
}
