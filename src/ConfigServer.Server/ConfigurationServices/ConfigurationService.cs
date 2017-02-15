using ConfigServer.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IConfigurationService
    {
        Task<ConfigInstance> GetAsync(Type type, ConfigurationIdentity id);
    }

    /// <summary>
    /// Builds Configuration from provider updating any properties coming from an external source
    /// </summary>
    internal class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationSetService configurationSetService;
        private readonly ConfigurationSetRegistry registry;

        public ConfigurationService(IConfigurationSetService configurationSetService, ConfigurationSetRegistry registry)
        {
            this.configurationSetService = configurationSetService;
            this.registry = registry;
        }

        public async Task<ConfigInstance> GetAsync(Type type, ConfigurationIdentity id)
        {
            var model = registry.GetConfigDefinition(type);
            var configurationSet =await configurationSetService.GetConfigurationSet(model.Type, id);
            
            return null;
        }
    }
}
