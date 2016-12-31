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
    }

    internal class ConfigInstanceRouter : IConfigInstanceRouter
    {
        private IConfigRepository configRepository;
        private readonly IEnumerable<ConfigurationModel> configModelCollection;
        private IConfigurationService configurationService;

        public ConfigInstanceRouter(IConfigRepository configRepository,IConfigurationService configurationService, ConfigurationSetRegistry configCollection)
        {
            this.configRepository = configRepository;
            this.configModelCollection = configCollection.SelectMany(s => s.Configs).ToList();
            this.configurationService = configurationService;
        }

        public async Task<ConfigInstance> GetConfigInstanceOrDefault(PathString path)
        {
            var configSetIds = await configRepository.GetClientsAsync();
            var configSetIdResult = configSetIds.TryMatchPath(c => c.ClientId, path);
            if (!configSetIdResult.HasResult)
                return null;
            var configModelResult = configModelCollection.TryMatchPath(s => s.Type.Name, configSetIdResult.RemainingPath);
            if (!configModelResult.HasResult)
                return null;
            var config = await configurationService.GetAsync(configModelResult.QueryResult.Type, new ConfigurationIdentity { ClientId = configSetIdResult.QueryResult.ClientId });
            return config;
        }
    }
}
