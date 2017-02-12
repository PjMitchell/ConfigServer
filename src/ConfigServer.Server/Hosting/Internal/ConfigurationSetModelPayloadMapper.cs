using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Server
{
    internal interface IConfigurationSetModelPayloadMapper
    {
        ConfigurationSetModelPayload Map(ConfigurationSetModel model, ConfigurationIdentity configIdentity);
    }

    internal class ConfigurationSetModelPayloadMapper : IConfigurationSetModelPayloadMapper
    {
        private readonly IOptionSetFactory optionSetFactory;
        private readonly IPropertyTypeProvider propertyTypeProvider;

        public ConfigurationSetModelPayloadMapper(IOptionSetFactory optionSetFactory, IPropertyTypeProvider propertyTypeProvider)
        {
            this.propertyTypeProvider = propertyTypeProvider;
            this.optionSetFactory = optionSetFactory;
        }

        public ConfigurationSetModelPayload Map(ConfigurationSetModel model, ConfigurationIdentity configIdentity)
        {
            return new ConfigurationSetModelPayload
            {
                ConfigurationSetId = model.ConfigSetType.Name,
                Name = model.Name,
                Description = model.Description,
                Config = BuildConfigs(model.Configs, configIdentity)
            };
        }



        private ConfigurationModelPayload Map(ConfigurationModel model, ConfigurationIdentity configIdentity)
        {
            return new ConfigurationModelPayload
            {
                Name = model.ConfigurationDisplayName,
                Description = model.ConfigurationDescription,
                Property = BuildProperties(model.ConfigurationProperties,configIdentity)
            };
        }

        private Dictionary<string, ConfigurationModelPayload> BuildConfigs(IEnumerable<ConfigurationModel> configs, ConfigurationIdentity configIdentity)
        {
            return configs.ToDictionary(c => c.Type.Name.ToLowerCamelCase(),v=> Map(v,configIdentity));
        }

        private Dictionary<string, ConfigurationPropertyPayload> BuildProperties(Dictionary<string, ConfigurationPropertyModelBase> arg, ConfigurationIdentity configIdentity)
        {
            return arg.ToDictionary(kvp => kvp.Key.ToLowerCamelCase(), kvp => BuildProperty(kvp.Value,configIdentity));
        }

        private ConfigurationPropertyPayload BuildProperty(ConfigurationPropertyModelBase value, ConfigurationIdentity configIdentity)
        {
            switch (value)
            {
                case ConfigurationPrimitivePropertyModel input:
                    return BuildProperty(input);
                case ConfigurationPropertyWithOptionsModelDefinition input:
                    return BuildProperty(input, configIdentity);
                case ConfigurationCollectionPropertyDefinition input:
                    return BuildProperty(input,configIdentity);
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
        private ConfigurationPropertyPayload BuildProperty(ConfigurationCollectionPropertyDefinition value, ConfigurationIdentity configIdentity)
        {
            return new ConfigurationPropertyPayload
            {
                PropertyName = value.ConfigurationPropertyName.ToLowerCamelCase(),
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = ConfigurationPropertyType.Collection,
                PropertyDescription = value.PropertyDescription,
                KeyPropertyName = value?.KeyPropertyName?.ToLowerCamelCase(),
                ChildProperty = BuildProperties(value.ConfigurationProperties,configIdentity)
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
