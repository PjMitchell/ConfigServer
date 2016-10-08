using System;
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

        public ConfigurationSetModelPayloadMapper(IServiceProvider serviceProvider)
        {
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

        private Dictionary<string, ConfigurationModelPayload> BuildConfigs(IEnumerable<ConfigurationModel> configs)
        {
            return configs.ToDictionary(c => c.Type.Name, BuildModelPayload);
        }

        private ConfigurationModelPayload BuildModelPayload(ConfigurationModel arg)
        {
            return new ConfigurationModelPayload
            {
                Name = arg.ConfigurationDisplayName,
                Description = arg.ConfigurationDescription,
                Property = BuildProperties(arg.ConfigurationProperties)
            };
        }

        private Dictionary<string, ConfigurationPropertyPayload> BuildProperties(Dictionary<string, ConfigurationPropertyModelBase> arg)
        {
            return arg.ToDictionary(kvp => kvp.Key, kvp => (ConfigurationPropertyPayload)BuildProperty((dynamic)kvp.Value));
        }

        private ConfigurationPropertyPayload BuildProperty(ConfigurationPrimitivePropertyModel value)
        {
            var propertyType = GetPropertyType(value);

            return new ConfigurationPropertyPayload
            {
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
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = GetPropertyType(value),
                PropertyDescription = value.PropertyDescription,
                Options = value.GetAvailableOptions(serviceProvider).ToDictionary(k=> k.Key, v=> v.DisplayValue)
            };
        }
        private ConfigurationPropertyPayload BuildProperty(ConfigurationCollectionPropertyDefinition value)
        {
            return new ConfigurationPropertyPayload
            {
                PropertyDisplayName = value.PropertyDisplayName,
                PropertyType = ConfigurationPropertyType.Collection,
                PropertyDescription = value.PropertyDescription,
                ChildProperty = BuildProperties(value.ConfigurationProperties)
            };
        }

        
        private Dictionary<string, string> BuildEnumOption(Type propertyType)
        {
            return Enum.GetNames(propertyType).ToDictionary(k => k);
        }

        private string GetPropertyType(ConfigurationPrimitivePropertyModel definition)
        {
            if (IsIntergerType(definition.PropertyType))
                return ConfigurationPropertyType.Interger;
            if (IsFloatType(definition.PropertyType))
                return ConfigurationPropertyType.Float;
            if (definition.PropertyType == typeof(string))
                return ConfigurationPropertyType.String;
            if (definition.PropertyType == typeof(bool))
                return ConfigurationPropertyType.Bool;
            if (definition.PropertyType == typeof(DateTime))
                return ConfigurationPropertyType.DateTime;
            if (typeof(Enum).IsAssignableFrom(definition.PropertyType))
                return ConfigurationPropertyType.DateTime;
            return ConfigurationPropertyType.Unacceptable;
        }

        private string GetPropertyType(ConfigurationPropertyWithOptionsModelDefinition definition)
        {
            if (definition.GetType().IsAssignableFrom(typeof(ConfigurationPropertyWithMultipleOptionsModelDefinition)))
                return ConfigurationPropertyType.MultipleOption;
            return ConfigurationPropertyType.Option;
        }

        private static bool IsIntergerType(Type type)
        {
            return type == typeof(int)
                || type == typeof(sbyte)
                || type == typeof(byte)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong);
        }

        private static bool IsFloatType(Type type)
        {
            return type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal);
        }
    }

    internal class ConfigurationPropertyType
    {
        public const string Interger = "Interger";
        public const string Float = "Float";
        public const string String = "String";
        public const string Bool = "Bool";
        public const string DateTime = "DateTime";
        public const string Enum = "Enum";

        public const string Option = "Option";
        public const string MultipleOption = "MultipleOption";

        public const string Collection = "Collection";

        public const string Unacceptable = "Unacceptable";


    }
}
