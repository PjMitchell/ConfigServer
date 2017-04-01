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
        Task<ConfigInstance> GetConfigInstanceOrDefault(PathString path);
        Task<ConfigInstance> GetConfigInstanceOrDefault(string clientId, string configId);
        Task<ConfigInstance> GetConfigInstanceOrDefault(ConfigurationClient client, string configId);
    }

    internal class ConfigInstanceRouter : IConfigInstanceRouter
    {
        private IConfigurationClientService configClientService;
        private readonly IEnumerable<ConfigurationModel> configModelCollection;
        private IConfigurationService configurationService;

        public ConfigInstanceRouter(IConfigurationClientService configClientService,IConfigurationService configurationService, ConfigurationSetRegistry configCollection)
        {
            this.configClientService = configClientService;
            this.configModelCollection = configCollection.SelectMany(s => s.Configs).ToList();
            this.configurationService = configurationService;
        }

        public Task<ConfigInstance> GetConfigInstanceOrDefault(PathString path)
        {
            var pathParams = path.ToPathParams();
            if (pathParams.Length != 2)
                return Task.FromResult<ConfigInstance>(null);
            return GetConfigInstanceOrDefault(pathParams[0], pathParams[1]);
        }

        public async Task<ConfigInstance> GetConfigInstanceOrDefault(string clientId, string configId)
        {
            var client = await configClientService.GetClientOrDefault(clientId);
            return await GetConfigInstanceOrDefault(client, configId);
        }


        public async Task<ConfigInstance> GetConfigInstanceOrDefault(ConfigurationClient client, string configId)
        {
            if (client == null)
                return null;
            var configModelResult = configModelCollection.SingleOrDefault(s => string.Equals(s.Type.Name, configId, StringComparison.OrdinalIgnoreCase));
            if (configModelResult == null)
                return null;
            var config = await configurationService.GetAsync(configModelResult.Type, new ConfigurationIdentity(client));
            return config;
        }
    }
}
