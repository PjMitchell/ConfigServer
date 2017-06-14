using ConfigServer.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IConfigInstanceRouter
    {
        Task<ConfigInstance> GetConfigInstanceOrDefault(ConfigurationClient client, string configId);
    }

    internal class ConfigInstanceRouter : IConfigInstanceRouter
    {
        private IConfigurationClientService configClientService;
        private readonly IEnumerable<ConfigurationModel> configModelCollection;
        private IConfigurationService configurationService;
        IConfigurationSetRegistry registry;

        public ConfigInstanceRouter(IConfigurationClientService configClientService,IConfigurationService configurationService, IConfigurationSetRegistry registry)
        {
            this.configClientService = configClientService;
            this.configModelCollection = registry.SelectMany(s => s.Configs).ToList();
            this.configurationService = configurationService;
            this.registry = registry;
        }

        public async Task<ConfigInstance> GetConfigInstanceOrDefault(ConfigurationClient client, string configId)
        {
            if (client == null)
                return null;
            var configModelResult = configModelCollection.SingleOrDefault(s => string.Equals(s.Type.Name, configId, StringComparison.OrdinalIgnoreCase));
            if (configModelResult == null)
                return null;
            var config = await configurationService.GetAsync(configModelResult.Type, new ConfigurationIdentity(client, registry.GetVersion()));
            return config;
        }
    }
}
