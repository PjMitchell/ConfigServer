using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    public class LocalConfigServerClient : IConfigServerClient
    {
        private readonly IConfigProvider configProvider;
        private readonly ConfigurationIdentity applicationId;
        public LocalConfigServerClient(IConfigProvider configProvider, string applicationId)
        {
            this.configProvider = configProvider;
            this.applicationId = new ConfigurationIdentity { ConfigSetId = applicationId };
        }

        public TConfig BuildConfig<TConfig>() where TConfig : class, new()
        {
            return configProvider.Get<TConfig>(applicationId).Configuration;
        }

        public object BuildConfig(Type type)
        {
            return configProvider.Get(type, applicationId).GetConfiguration();
        }

        public Task<TConfig> BuildConfigAsync<TConfig>() where TConfig : class, new()
        {
            return Task.FromResult(BuildConfig<TConfig>());
        }

        public Task<object> BuildConfigAsync(Type type)
        {
            return Task.FromResult(BuildConfig(type));
        }
    }
}
