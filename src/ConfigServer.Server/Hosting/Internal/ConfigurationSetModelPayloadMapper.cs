﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Server
{
    internal interface IConfigurationSetModelPayloadMapper
    {
        ConfigurationSetModelPayload Map(ConfigurationSetModel model);
    }

    internal class ConfigurationSetModelPayloadMapper : IConfigurationSetModelPayloadMapper
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IPropertyTypeProvider propertyTypeProvider;

        public ConfigurationSetModelPayloadMapper(IServiceProvider serviceProvider, IPropertyTypeProvider propertyTypeProvider)
        {
            this.propertyTypeProvider = propertyTypeProvider;
            this.serviceProvider = serviceProvider;
        }

        public ConfigurationSetModelPayload Map(ConfigurationSetModel model)
        {
            return new ConfigurationSetModelPayload
            {
                ConfigurationSetId = model.ConfigSetType.Name,
                Name = model.Name,
                Description = model.Description,
                Config = BuildConfigs(model.Configs)
            };
        }



        private ConfigurationModelPayload Map(ConfigurationModel model)
        {
            return new ConfigurationModelPayload
            {
                Name = model.ConfigurationDisplayName,
                Description = model.ConfigurationDescription,
                Property = BuildProperties(model.ConfigurationProperties)
            };
        }

        private Dictionary<string, ConfigurationModelPayload> BuildConfigs(IEnumerable<ConfigurationModel> configs)
        {
            return configs.ToDictionary(c => c.Type.Name.ToLowerCamelCase(), Map);
        }

        private Dictionary<string, ConfigurationPropertyPayload> BuildProperties(Dictionary<string, ConfigurationPropertyModelBase> arg)
        {
            return arg.ToDictionary(kvp => kvp.Key.ToLowerCamelCase(), kvp => (ConfigurationPropertyPayload)BuildProperty((dynamic)kvp.Value));
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

        private ConfigurationPropertyPayload BuildProperty(ConfigurationPropertyWithOptionsModelDefinition value)
        {
            return new ConfigurationPropertyPayload
            {
                PropertyName = value.ConfigurationPropertyName.ToLowerCamelCase(),
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = propertyTypeProvider.GetPropertyType(value),
                PropertyDescription = value.PropertyDescription,
                Options = value.GetAvailableOptions(serviceProvider).ToDictionary(k=> k.Key, v=> v.DisplayValue)
            };
        }
        private ConfigurationPropertyPayload BuildProperty(ConfigurationCollectionPropertyDefinition value)
        {
            return new ConfigurationPropertyPayload
            {
                PropertyName = value.ConfigurationPropertyName.ToLowerCamelCase(),
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = ConfigurationPropertyType.Collection,
                PropertyDescription = value.PropertyDescription,
                ChildProperty = BuildProperties(value.ConfigurationProperties)
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
