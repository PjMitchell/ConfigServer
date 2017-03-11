using ConfigServer.Core;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IConfigurationUpdatePayloadMapper
    {
        Task<ConfigInstance> UpdateConfigurationInstance(ConfigInstance original, JContainer newEditPayload, ConfigurationSetModel model);
    }

    internal class ConfigurationUpdatePayloadMapper : IConfigurationUpdatePayloadMapper
    {
        readonly IPropertyTypeProvider propertyTypeProvider;
        readonly IOptionSetFactory optionSetFactory;
        readonly IConfigurationSetService configurationSetService;

        public ConfigurationUpdatePayloadMapper(IOptionSetFactory optionSetFactory, IPropertyTypeProvider propertyTypeProvider, IConfigurationSetService configurationSetService)
        {
            this.propertyTypeProvider = propertyTypeProvider;
            this.optionSetFactory = optionSetFactory;
            this.configurationSetService = configurationSetService;
        }

        public async Task<ConfigInstance> UpdateConfigurationInstance(ConfigInstance original, JContainer newEditPayload, ConfigurationSetModel model)
        {
            var configModel = model.Configs.Single(s => s.Type == original.ConfigType);
            var identity = new ConfigurationIdentity(original.ClientId);
            var configurationSets = await GetRequiredConfiguration(model, identity);
            var newConfig = UpdateObject(original, newEditPayload, configModel, identity, configurationSets);
            original.SetConfiguration(newConfig);
            return original;
        }

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
                var obj = optionModel.NewItemInstance();
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
            var propertyValue = source.GetValue(propertyModel.ConfigurationPropertyName.ToLowerCamelCase());
            if (propertyModel is ConfigurationPrimitivePropertyModel primativeModel && primativeModel.ValidationRules.IsRequired && propertyValue.Type == JTokenType.Null)
                throw new ConfigModelParsingException($"{propertyModel.PropertyDisplayName} is Required");
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
            if (propertyModel is ConfigurationPropertyWithMultipleOptionValuesModelDefinition valueDefintion)
                return GetConfigMultipleOptionPropertyValueFromInput(source, valueDefintion, configIdentity, requiredConfigurationSets);
            return GetConfigMultipleOptionPropertyValueFromInput(source, propertyModel, configIdentity, requiredConfigurationSets);
        }

        private object GetConfigMultipleOptionPropertyValueFromInput(JObject source, ConfigurationPropertyWithMultipleOptionValuesModelDefinition propertyModel, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            var collectionBuilder = propertyModel.GetCollectionBuilder();
            var optionSet = optionSetFactory.Build(propertyModel, configIdentity, requiredConfigurationSets);
            foreach (var key in source.GetValue(propertyModel.ConfigurationPropertyName.ToLowerCamelCase()).Select(s => s.ToObject(propertyModel.PropertyType)))
            {
                if (optionSet.ContainsKey(key))
                    collectionBuilder.Add(key);
            }
            return collectionBuilder.Collection;
        }

        private object GetConfigMultipleOptionPropertyValueFromInput(JObject source, IMultipleOptionPropertyDefinition propertyModel, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
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
                var config = propertyModel.NewItemInstance();
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
            foreach (var type in requiredConfigurationSetTypes)
            {
                configurationSet[i] = await configurationSetService.GetConfigurationSet(type, identity);
                i++;
            }
            return configurationSet;
        }
    }
}
