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
            var configurationSets = await GetRequiredConfiguration(model, configIdentity);
            return new ConfigurationSetModelPayload
            {
                ConfigurationSetId = model.ConfigSetType.Name,
                Name = model.Name,
                Description = model.Description,
                Config = BuildConfigs(model.Configs, configIdentity, configurationSets)
            };
        }

        private ConfigurationModelPayload Map(ConfigurationModel model, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            return new ConfigurationModelPayload
            {
                Name = model.ConfigurationDisplayName,
                Description = model.ConfigurationDescription,
                IsOption = model is ConfigurationOptionModel,
                KeyPropertyName = model?.KeyPropertyName?.ToLowerCamelCase(),
                Property = BuildProperties(model.ConfigurationProperties,configIdentity,requiredConfigurationSets)
            };
        }

        private Dictionary<string, ConfigurationModelPayload> BuildConfigs(IEnumerable<ConfigurationModel> configs, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            var result = new Dictionary<string, ConfigurationModelPayload>();
            foreach (var config in configs.Where(w=> !w.IsReadOnly))
            {
                result.Add(config.Type.Name.ToLowerCamelCase(), Map(config, configIdentity, requiredConfigurationSets));
            }
            return result;
        }

        private Dictionary<string, ConfigurationPropertyPayload> BuildProperties(Dictionary<string, ConfigurationPropertyModelBase> arg, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            var result = new Dictionary<string, ConfigurationPropertyPayload>();
            foreach (var config in arg)
            {
                result.Add(config.Key.ToLowerCamelCase(), BuildProperty(config.Value, configIdentity, requiredConfigurationSets));
            }
            return result;
        }

        private ConfigurationPropertyPayload BuildProperty(ConfigurationPropertyModelBase value, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            switch (value)
            {
                case ConfigurationPrimitivePropertyModel input:
                    return BuildProperty(input);
                case IOptionPropertyDefinition input:
                    return BuildProperty(input, configIdentity, requiredConfigurationSets);
                case ConfigurationCollectionPropertyDefinition input:
                    return BuildProperty(input, configIdentity,requiredConfigurationSets);
                case ConfigurationClassPropertyDefinition input:
                    return BuildProperty(input, configIdentity, requiredConfigurationSets);
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

        private ConfigurationPropertyPayload BuildProperty(IOptionPropertyDefinition value, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            var optionSet = optionSetFactory.Build(value, configIdentity, requiredConfigurationSets);
            return new ConfigurationPropertyPayload
            {
                PropertyName = value.ConfigurationPropertyName.ToLowerCamelCase(),
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = propertyTypeProvider.GetPropertyType(value),
                PropertyDescription = value.PropertyDescription,
                Options = optionSet.OptionSelections.ToDictionary(k=> k.Key, v=> v.DisplayValue)
            };
        }

        private ConfigurationPropertyPayload BuildProperty(ConfigurationCollectionPropertyDefinition value, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            return new ConfigurationPropertyPayload
            {
                PropertyName = value.ConfigurationPropertyName.ToLowerCamelCase(),
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = ConfigurationPropertyType.Collection,
                PropertyDescription = value.PropertyDescription,
                KeyPropertyName = value?.KeyPropertyName?.ToLowerCamelCase(),
                ChildProperty = BuildProperties(value.ConfigurationProperties,configIdentity,requiredConfigurationSets)
            };
        }

        private ConfigurationPropertyPayload BuildProperty(ConfigurationClassPropertyDefinition value, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> requiredConfigurationSets)
        {
            return new ConfigurationPropertyPayload
            {
                PropertyName = value.ConfigurationPropertyName.ToLowerCamelCase(),
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = ConfigurationPropertyType.Class,
                PropertyDescription = value.PropertyDescription,
                ChildProperty = BuildProperties(value.ConfigurationProperties, configIdentity, requiredConfigurationSets)
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
