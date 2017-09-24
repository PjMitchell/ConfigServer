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
        private readonly IConfigurationClientService configurationClientService;
        private readonly IConfigurationModelRegistry registry;

        public LocalConfigServerClient(IConfigProvider configProvider,IConfigurationClientService configurationClientService, IConfigurationModelRegistry registry, LocalServerClientOptions options)
        {
            this.configProvider = configProvider;
            this.applicationId = options.ApplicationId;
            this.configurationClientService = configurationClientService;
            this.registry = registry;
        }

        public Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>() where TConfig : class, new()
        {
            return GetCollectionConfigAsync<TConfig>(applicationId);
        }

        public async Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>(string clientId) where TConfig : class, new()
        {
            var client = await configurationClientService.GetClientOrDefault(clientId);
            var config = await configProvider.GetCollectionAsync<TConfig>(new ConfigurationIdentity(client, registry.GetVersion())).ConfigureAwait(false);
            return config;
        }

        public Task<TConfig> GetConfigAsync<TConfig>() where TConfig : class, new()
        {
            return GetConfigAsync<TConfig>(applicationId);
        }

        public async Task<TConfig> GetConfigAsync<TConfig>(string clientId) where TConfig : class, new()
        {
            var client = await configurationClientService.GetClientOrDefault(applicationId);
            var config = await configProvider.GetAsync<TConfig>(new ConfigurationIdentity(client, registry.GetVersion())).ConfigureAwait(false);
            return config.Configuration;
        }

        public Task<object> GetConfigAsync(Type type)
        {
            return GetConfigAsync(type, applicationId);
        }
        
        public async Task<object> GetConfigAsync(Type type, string clientId)
        {
            var client = await configurationClientService.GetClientOrDefault(applicationId);
            var config = await configProvider.GetAsync(type, new ConfigurationIdentity(client, registry.GetVersion())).ConfigureAwait(false);
            return config.GetConfiguration();
        }
    }
}
