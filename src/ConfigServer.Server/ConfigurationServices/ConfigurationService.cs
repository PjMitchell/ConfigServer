﻿using ConfigServer.Core;
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
        private readonly ConfigurationModelRegistry registry;

        public ConfigurationService(IConfigurationSetService configurationSetService, ConfigurationModelRegistry registry)
        {
            this.configurationSetService = configurationSetService;
            this.registry = registry;
        }

        public async Task<ConfigInstance> GetAsync(Type type, ConfigurationIdentity id)
        {
            var setModel = registry.GetConfigSetForConfig(type);
            var model = setModel.Get(type);

            var configurationSet =await configurationSetService.GetConfigurationSet(setModel.ConfigSetType, id);
            var config = model.GetConfigInstanceFromConfigurationSet(configurationSet);
            return config;
        }
    }
}
