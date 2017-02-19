using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IConfigurationSetModelPayloadMapper
    {
        Task<ConfigurationSetModelPayload> Map(ConfigurationSetModel model, ConfigurationIdentity configIdentity);
    }

    internal class ConfigurationSetModelPayloadMapper : IConfigurationSetModelPayloadMapper
    {
        private readonly IOptionSetFactory optionSetFactory;
        private readonly IPropertyTypeProvider propertyTypeProvider;
        readonly IConfigurationSetService configurationSetService;

        public ConfigurationSetModelPayloadMapper(IOptionSetFactory optionSetFactory, IConfigurationSetService configurationSetService, IPropertyTypeProvider propertyTypeProvider)
        {
            this.propertyTypeProvider = propertyTypeProvider;
            this.optionSetFactory = optionSetFactory;
            this.configurationSetService = configurationSetService;
        }

        public async Task<ConfigurationSetModelPayload> Map(ConfigurationSetModel model, ConfigurationIdentity configIdentity)
        {
            return new ConfigurationSetModelPayload
            {
                ConfigurationSetId = model.ConfigSetType.Name,
                Name = model.Name,
                Description = model.Description,
                Config = await BuildConfigs(model.Configs, configIdentity)
            };
        }



        private async Task<ConfigurationModelPayload> Map(ConfigurationModel model, ConfigurationIdentity configIdentity)
        {
            return new ConfigurationModelPayload
            {
                Name = model.ConfigurationDisplayName,
                Description = model.ConfigurationDescription,
                Property = await BuildProperties(model.ConfigurationProperties,configIdentity)
            };
        }

        private async Task<Dictionary<string, ConfigurationModelPayload>> BuildConfigs(IEnumerable<ConfigurationModel> configs, ConfigurationIdentity configIdentity)
        {
            var result = new Dictionary<string, ConfigurationModelPayload>();
            foreach (var config in configs)
            {
                result.Add(config.Type.Name.ToLowerCamelCase(), await Map(config, configIdentity));
            }
            return result;
        }

        private async Task<Dictionary<string, ConfigurationPropertyPayload>> BuildProperties(Dictionary<string, ConfigurationPropertyModelBase> arg, ConfigurationIdentity configIdentity)
        {
            var result = new Dictionary<string, ConfigurationPropertyPayload>();
            foreach (var config in arg)
            {
                result.Add(config.Key.ToLowerCamelCase(), await BuildProperty(config.Value, configIdentity));
            }
            return result;
        }

        private async Task<ConfigurationPropertyPayload> BuildProperty(ConfigurationPropertyModelBase value, ConfigurationIdentity configIdentity)
        {
            switch (value)
            {
                case ConfigurationPrimitivePropertyModel input:
                    return BuildProperty(input);
                case ConfigurationPropertyWithOptionsModelDefinition input:
                    return BuildProperty(input, configIdentity);
                case ConfigurationPropertyWithConfigSetOptionsModelDefinition input:
                    return await BuildProperty(input, configIdentity);
                case ConfigurationCollectionPropertyDefinition input:
                    return await BuildProperty(input,configIdentity);
                default:
                    throw new InvalidOperationException($"Could not handle ConfigurationPropertyModelBase of type {value.GetType().Name}");
            }
        }

        private ConfigurationPropertyPayload BuildProperty(ConfigurationPrimitivePropertyModel value)
        {
            var propertyType = propertyTypeProvider.GetPropertyType(value);

            return new ConfigurationPropertyPayload
            {
                PropertyName = value.ConfigurationPropertyName.ToLowerCamelCase(),
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = propertyType,
                ValidationDefinition = value.ValidationRules,
                PropertyDescription = value.PropertyDescription,
                Options = propertyType == ConfigurationPropertyType.Enum? BuildEnumOption(value.PropertyType) : new Dictionary<string, string>()
            };
        }

        private ConfigurationPropertyPayload BuildProperty(ConfigurationPropertyWithOptionsModelDefinition value, ConfigurationIdentity configIdentity)
        {
            var optionSet = optionSetFactory.Build(value, configIdentity);
            return new ConfigurationPropertyPayload
            {
                PropertyName = value.ConfigurationPropertyName.ToLowerCamelCase(),
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = propertyTypeProvider.GetPropertyType(value),
                PropertyDescription = value.PropertyDescription,
                Options = optionSet.OptionSelections.ToDictionary(k=> k.Key, v=> v.DisplayValue)
            };
        }

        private async Task<ConfigurationPropertyPayload> BuildProperty(ConfigurationPropertyWithConfigSetOptionsModelDefinition value, ConfigurationIdentity configIdentity)
        {
            var configurationSet = await configurationSetService.GetConfigurationSet(value.ConfigurationSetType, configIdentity);
            var optionSet = optionSetFactory.Build(value, configurationSet);
            return new ConfigurationPropertyPayload
            {
                PropertyName = value.ConfigurationPropertyName.ToLowerCamelCase(),
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = propertyTypeProvider.GetPropertyType(value),
                PropertyDescription = value.PropertyDescription,
                Options = optionSet.OptionSelections.ToDictionary(k => k.Key, v => v.DisplayValue)
            };
        }

        private async Task<ConfigurationPropertyPayload> BuildProperty(ConfigurationCollectionPropertyDefinition value, ConfigurationIdentity configIdentity)
        {
            return new ConfigurationPropertyPayload
            {
                PropertyName = value.ConfigurationPropertyName.ToLowerCamelCase(),
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = ConfigurationPropertyType.Collection,
                PropertyDescription = value.PropertyDescription,
                KeyPropertyName = value?.KeyPropertyName?.ToLowerCamelCase(),
                ChildProperty = await BuildProperties(value.ConfigurationProperties,configIdentity)
            };
        }

        
        private Dictionary<string, string> BuildEnumOption(Type propertyType)
        {
            return GetEnumValues(propertyType).ToDictionary(k => k.Key.ToString(), v=> v.Value);
        }

        private IEnumerable<KeyValuePair<int,string>> GetEnumValues(Type propertyType)
        {
            foreach(var obj in Enum.GetValues(propertyType))
            {
                yield return new KeyValuePair<int, string>((int)obj, obj.ToString());
            }
        }
    }
}
