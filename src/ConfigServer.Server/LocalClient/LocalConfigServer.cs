using ConfigServer.Core;
using System;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class LocalConfigServerClient : IConfigServerClient
    {
        private readonly IConfigProvider configProvider;
        private readonly ConfigurationIdentity applicationId;
        public LocalConfigServerClient(IConfigProvider configProvider, string applicationId)
        {
            this.configProvider = configProvider;
            this.applicationId = new ConfigurationIdentity { ClientId = applicationId };
        }

        public async Task<TConfig> BuildConfigAsync<TConfig>() where TConfig : class, new()
        {
            var config = await configProvider.GetAsync<TConfig>(applicationId);
            return config.Configuration;
        }

        public async Task<object> BuildConfigAsync(Type type)
        {
            var config = await configProvider.GetAsync(type,applicationId);
            return config.GetConfiguration();
        }
    }
}
