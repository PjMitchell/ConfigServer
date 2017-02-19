using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Dynamic;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IConfigurationEditPayloadMapper
    {
        object MapToEditConfig(ConfigInstance config, ConfigurationSetModel model);
        Task<ConfigInstance> UpdateConfigurationInstance(ConfigInstance original, JObject newEditPayload, ConfigurationSetModel model);
    }


    internal class ConfigurationEditPayloadMapper : IConfigurationEditPayloadMapper
    {
        readonly IPropertyTypeProvider propertyTypeProvider;
        readonly IOptionSetFactory optionSetFactory;
        readonly IConfigurationSetService configurationSetService;

        public ConfigurationEditPayloadMapper(IOptionSetFactory optionSetFactory, IPropertyTypeProvider propertyTypeProvider, IConfigurationSetService configurationSetService)
        {
            this.propertyTypeProvider = propertyTypeProvider;
            this.optionSetFactory = optionSetFactory;
            this.configurationSetService = configurationSetService;
        }

        public object MapToEditConfig(ConfigInstance config, ConfigurationSetModel model)
        {
            var configModel = model.Configs.Single(s => s.Type == config.ConfigType);
            var source = config.GetConfiguration();
            return BuildObject(source, configModel.ConfigurationProperties);
        }

        public async Task<ConfigInstance> UpdateConfigurationInstance(ConfigInstance original, JObject newEditPayload, ConfigurationSetModel model)
        {
            var configModel = model.Configs.Single(s => s.Type == original.ConfigType);
            var identity = new ConfigurationIdentity(original.ClientId);
            var configurationSets = await GetRequiredConfiguration(model, identity);
            var newConfig = UpdateObject(original.ConstructNewConfiguration(), newEditPayload,configModel.ConfigurationProperties, identity, configurationSets);
            original.SetConfiguration(newConfig);
            return original; 
        }

        private object BuildObject(object source, Dictionary<string, ConfigurationPropertyModelBase> properties)
        {
            IDictionary<string, object> obj = new ExpandoObject();
            foreach (var property in properties)
            {
                var propertyType = propertyTypeProvider.GetPropertyType(property.Value);
                obj[property.Key] = GetPropertyValueFromConfig(propertyType, source, property.Value);
            }
            return obj;
        }

        private object GetPropertyValueFromConfig(string propertyType, object source, ConfigurationPropertyModelBase propertyModel)
        {
            switch (propertyType)
            {
                case ConfigurationPropertyType.Option:
                    return GetOptionPropertyValue(source, propertyModel);
                case ConfigurationPropertyType.MultipleOption:
                    return GetPropertyValue(source, (ConfigurationPropertyWithMultipleOptionsModelDefinition)propertyModel);
                case ConfigurationPropertyType.Collection:
                    return GetPropertyValue(source, (ConfigurationCollectionPropertyDefinition)propertyModel);
                default:
                    return propertyModel.GetPropertyValue(source);                        

            }
        }

        private object GetOptionPropertyValue(object source, ConfigurationPropertyModelBase propertyModel)
        {
            var value = propertyModel.GetPropertyValue(source);
            if (value == null)
                return null;
            if(propertyModel is ConfigurationPropertyWithOptionsModelDefinition optionModel)
                return optionModel.GetKeyFromObject(value);
            return optionSetFactory.GetKeyFromObject(value, (ConfigurationPropertyWithConfigSetOptionsModelDefinition)propertyModel);
        }

        private object GetPropertyValue(object source, ConfigurationPropertyWithMultipleOptionsModelDefinition propertyModel)
        {
            var collection = propertyModel.GetPropertyValue(source) as IEnumerable ?? new List<object>();
            var result = new List<string>();
            foreach(var item in collection)
            {
                var itemValue = propertyModel.GetKeyFromObject(item);
                result.Add(itemValue);
            }

            return result;
        }

        private object GetPropertyValue(object source, ConfigurationCollectionPropertyDefinition propertyModel)
        {
            var collection = propertyModel.GetPropertyValue(source) as IEnumerable ?? new List<object>();

            var result = new List<object>();
            foreach (var item in collection)
            {
                var itemValue = BuildObject(item, propertyModel.ConfigurationProperties);
                result.Add(itemValue);
            }

            return result;
        }

        private object UpdateObject(object target, JObject source, Dictionary<string, ConfigurationPropertyModelBase> properties, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            foreach (var property in properties)
            {
                var propertyType = propertyTypeProvider.GetPropertyType(property.Value);
                property.Value.SetPropertyValue(target, GetConfigPropertyValueFromInput(propertyType, source, property.Value, configIdentity, requiredConfigurationSets));
            }
            return target;
        }

        private object GetConfigPropertyValueFromInput(string propertyType, JObject source, ConfigurationPropertyModelBase propertyModel, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            switch (propertyType)
            {
                case ConfigurationPropertyType.Option:
                    return GetOptionConfigPropertyValueFromInput(source, propertyModel, configIdentity, requiredConfigurationSets);
                case ConfigurationPropertyType.MultipleOption:
                    return GetConfigPropertyValueFromInput(source, (ConfigurationPropertyWithMultipleOptionsModelDefinition)propertyModel, configIdentity);
                case ConfigurationPropertyType.Collection:
                    return GetConfigPropertyValueFromInput(source, (ConfigurationCollectionPropertyDefinition)propertyModel, configIdentity, requiredConfigurationSets);
                default:
                    return GetConfigPropertyValueFromInput(source, propertyModel);

            }
        }

        private object GetConfigPropertyValueFromInput(JObject source, ConfigurationPropertyModelBase propertyModel)
        {
            var result = source.GetValue(propertyModel.ConfigurationPropertyName.ToLowerCamelCase())?.ToObject(propertyModel.PropertyType);
            return result;
        }

        private object GetOptionConfigPropertyValueFromInput(JObject source, ConfigurationPropertyModelBase propertyModel, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            var key = source.GetValue(propertyModel.ConfigurationPropertyName.ToLowerCamelCase()).ToObject<string>();
            IOptionSet optionSet;
            if(propertyModel is ConfigurationPropertyWithOptionsModelDefinition optionProperty)
                optionSet = optionSetFactory.Build(optionProperty, configIdentity);
            else
                optionSet = optionSetFactory.Build((ConfigurationPropertyWithConfigSetOptionsModelDefinition)propertyModel, requiredConfigurationSets);
            object option = null;
            optionSet.TryGetValue(key, out option);
            return option;
        }

        private object GetConfigPropertyValueFromInput(JObject source, ConfigurationPropertyWithMultipleOptionsModelDefinition propertyModel, ConfigurationIdentity configIdentity)
        {
            var collectionBuilder = propertyModel.GetCollectionBuilder();
            var optionSet = optionSetFactory.Build(propertyModel, configIdentity);
            foreach (var key in source.GetValue(propertyModel.ConfigurationPropertyName.ToLowerCamelCase()).Select(s => s.ToObject<string>()))
            {
                object option = null;
                if (optionSet.TryGetValue(key, out option))
                    collectionBuilder.Add(option);
            }
            return collectionBuilder.Collection;
        }

        private object GetConfigPropertyValueFromInput(JObject source, ConfigurationCollectionPropertyDefinition propertyModel, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            var collectionBuilder = propertyModel.GetCollectionBuilder();
            foreach (var item in source.GetValue(propertyModel.ConfigurationPropertyName.ToLowerCamelCase()))
            {
                var config = collectionBuilder.IntializeNewItem();
                config = UpdateObject(config, (JObject)item, propertyModel.ConfigurationProperties, configIdentity, requiredConfigurationSets);
                collectionBuilder.Add(config);
            }
            return collectionBuilder.Collection;
        }

        private async Task<IEnumerable<ConfigurationSet>> GetRequiredConfiguration(ConfigurationSetModel model, ConfigurationIdentity identity)
        {
            var requiredConfigurationSetTypes = model.GetDependencies()
                .Select(s => s.ConfigurationSet)
                .Distinct()
                .ToArray();
            var configurationSet = new ConfigurationSet[requiredConfigurationSetTypes.Length];
            var i = 0;
            foreach(var type in requiredConfigurationSetTypes)
            {
                configurationSet[i] = await configurationSetService.GetConfigurationSet(type, identity);
                i++;
            }
            return configurationSet;
        }
    }
}
