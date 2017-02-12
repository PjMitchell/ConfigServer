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
        private readonly IConfigProvider configProvider;
        private readonly IOptionSetFactory optionSetFactory;
        private readonly ConfigurationSetRegistry registry;

        public ConfigurationService(IConfigProvider configProvider, IOptionSetFactory optionSetFactory, ConfigurationSetRegistry registry)
        {
            this.configProvider = configProvider;
            this.optionSetFactory = optionSetFactory;
            this.registry = registry;
        }

        public async Task<ConfigInstance> GetAsync(Type type, ConfigurationIdentity id)
        {
            var model = registry.GetConfigDefinition(type);
            var configInstance =await configProvider.GetAsync(type, id);
            UpdateOptions(configInstance.GetConfiguration(), model.ConfigurationProperties,id);
            return configInstance;
        }

        private void UpdateOptions(object source, Dictionary<string, ConfigurationPropertyModelBase> models, ConfigurationIdentity configIdentity)
        {
            foreach(var model in models)
            {
                UpdateOptions(source, model.Value, configIdentity);
            }
        }
        private void UpdateOptions(object source, ConfigurationPropertyModelBase model, ConfigurationIdentity configIdentity)
        {

            switch (model)
            {
                case ConfigurationPropertyWithMultipleOptionsModelDefinition optionsProperty:
                    UpdateOptions(source, optionsProperty,configIdentity);
                    break;
                case ConfigurationPropertyWithOptionsModelDefinition optionProperty:
                    UpdateOptions(source, optionProperty,configIdentity);
                    break;
                case ConfigurationCollectionPropertyDefinition collectionProperty:
                    UpdateOptions(source, collectionProperty,configIdentity);
                    break;
                default:
                    break;
            }
        }
        private void UpdateOptions(object source, ConfigurationPropertyWithOptionsModelDefinition model, ConfigurationIdentity configIdentity)
        {
            var optionSet = optionSetFactory.Build(model,configIdentity);
            var orignal = model.GetPropertyValue(source);
            optionSet.TryGetValue(orignal, out var actualValue);
            model.SetPropertyValue(source,actualValue);
        }

        private void UpdateOptions(object source, ConfigurationPropertyWithMultipleOptionsModelDefinition model, ConfigurationIdentity configIdentity)
        {
            var optionSet = optionSetFactory.Build(model,configIdentity);
            var collectionBuilder = model.GetCollectionBuilder();
            var items = model.GetPropertyValue(source) as IEnumerable;
            foreach (var item in items?? Enumerable.Empty<object>())
            {
                if(optionSet.TryGetValue(item, out var actualValue))
                    collectionBuilder.Add(actualValue);
            }
                        
            model.SetPropertyValue(source, collectionBuilder.Collection);
        }

        private void UpdateOptions(object source, ConfigurationCollectionPropertyDefinition model, ConfigurationIdentity configIdentity)
        {
            var items = model.GetPropertyValue(source) as IEnumerable;
            if(items == null)
            {
                var collectionBuilder = model.GetCollectionBuilder();
                model.SetPropertyValue(source, collectionBuilder.Collection);
                return;
            }
            foreach(var item in items)
            {
                UpdateOptions(item, model.ConfigurationProperties,configIdentity);
            }
        }
    }
}
