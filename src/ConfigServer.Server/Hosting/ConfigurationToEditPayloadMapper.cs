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
        Task<ConfigInstance> UpdateConfigurationInstance(ConfigInstance original, JContainer newEditPayload, ConfigurationSetModel model);

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
            return BuildObject(source, configModel);
        }

        public async Task<ConfigInstance> UpdateConfigurationInstance(ConfigInstance original, JContainer newEditPayload, ConfigurationSetModel model)
        {
            var configModel = model.Configs.Single(s => s.Type == original.ConfigType);
            var identity = new ConfigurationIdentity(original.ClientId);
            var configurationSets = await GetRequiredConfiguration(model, identity);
            var newConfig = UpdateObject(original, newEditPayload,configModel, identity, configurationSets);
            original.SetConfiguration(newConfig);
            return original; 
        }


        #region BuildObject
        private object BuildObject(object source, ConfigurationModel model)
        {
            if (model is ConfigurationOptionModel optionModel)
                return BuildObject(source, optionModel);
            return BuildObject(source, model.ConfigurationProperties);
        }

        private object BuildObject(object source, ConfigurationOptionModel model)
        {
            var collection = source as IEnumerable ?? new List<object>();

            var result = new List<object>();
            foreach (var item in collection)
            {
                var itemValue = BuildObject(item, model.ConfigurationProperties);
                result.Add(itemValue);
            }

            return result;
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
                    return GetOptionPropertyValue(source, (IOptionPropertyDefinition)propertyModel);
                case ConfigurationPropertyType.MultipleOption:
                    return GetPropertyValue(source, (IMultipleOptionPropertyDefinition)propertyModel);
                case ConfigurationPropertyType.Collection:
                    return GetPropertyValue(source, (ConfigurationCollectionPropertyDefinition)propertyModel);
                default:
                    return propertyModel.GetPropertyValue(source);                        

            }
        }

        private object GetOptionPropertyValue(object source, IOptionPropertyDefinition propertyModel)
        {
            var value = propertyModel.GetPropertyValue(source);
            if (value == null)
                return null;
            return optionSetFactory.GetKeyFromObject(value, propertyModel);
        }

        private object GetPropertyValue(object source, IMultipleOptionPropertyDefinition propertyModel)
        {
            var collection = propertyModel.GetPropertyValue(source) as IEnumerable ?? new List<object>();
            var result = new List<string>();
            foreach(var item in collection)
            {
                var itemValue = optionSetFactory.GetKeyFromObject(item, propertyModel);
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
        #endregion



        private object UpdateObject(ConfigInstance original, JContainer newEditPayload, ConfigurationModel model, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            if (original is ConfigCollectionInstance collection)
                return UpdateObject(collection, (JArray)newEditPayload, (ConfigurationOptionModel)model, configIdentity, requiredConfigurationSets);
            else
                return UpdateObject(original.ConstructNewConfiguration(), (JObject)newEditPayload, model.ConfigurationProperties, configIdentity, requiredConfigurationSets);
        }

        private object UpdateObject(ConfigCollectionInstance target, JArray source, ConfigurationOptionModel optionModel, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            var collectionBuilder = target.CreateCollectionBuilder();
            foreach (var item in source)
            {
                var obj = collectionBuilder.IntializeNewItem();
                var itemToAdd = UpdateObject(obj, (JObject)item, optionModel.ConfigurationProperties, configIdentity, requiredConfigurationSets);
                collectionBuilder.Add(itemToAdd);
            }
            return collectionBuilder.Collection;
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
                    return GetOptionConfigPropertyValueFromInput(source, (IOptionPropertyDefinition)propertyModel, configIdentity, requiredConfigurationSets);
                case ConfigurationPropertyType.MultipleOption:
                    return GetConfigPropertyValueFromInput(source, (IMultipleOptionPropertyDefinition)propertyModel, configIdentity, requiredConfigurationSets);
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

        private object GetOptionConfigPropertyValueFromInput(JObject source, IOptionPropertyDefinition propertyModel, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            var key = source.GetValue(propertyModel.ConfigurationPropertyName.ToLowerCamelCase()).ToObject<string>();
            IOptionSet optionSet = optionSetFactory.Build(propertyModel, configIdentity, requiredConfigurationSets);
            object option = null;
            optionSet.TryGetValue(key, out option);
            return option;
        }

        private object GetConfigPropertyValueFromInput(JObject source, IMultipleOptionPropertyDefinition propertyModel, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            var collectionBuilder = propertyModel.GetCollectionBuilder();
            var optionSet = optionSetFactory.Build(propertyModel, configIdentity, requiredConfigurationSets);
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
