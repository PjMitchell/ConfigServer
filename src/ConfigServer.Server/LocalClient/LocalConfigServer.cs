using ConfigServer.Core;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class LocalConfigServerClient : IConfigServer
    {
        private readonly IConfigProvider configProvider;
        private readonly ConfigurationIdentity applicationId;
        public LocalConfigServerClient(IConfigProvider configProvider, string applicationId)
        {
            this.configProvider = configProvider;
            this.applicationId = new ConfigurationIdentity(applicationId);
        }

        public async Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>() where TConfig : class, new()
        {
            var config = await configProvider.GetCollectionAsync<TConfig>(applicationId).ConfigureAwait(false);
            return config;
        }

        public async Task<TConfig> GetConfigAsync<TConfig>() where TConfig : class, new()
        {
            var config = await configProvider.GetAsync<TConfig>(applicationId).ConfigureAwait(false);
            return config.Configuration;
        }

        public async Task<object> GetConfigAsync(Type type)
        {
            var config = await configProvider.GetAsync(type,applicationId).ConfigureAwait(false);
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
    }
}
