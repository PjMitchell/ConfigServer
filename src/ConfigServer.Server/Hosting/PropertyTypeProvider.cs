using System;
using System.Reflection;
namespace ConfigServer.Server
{
    internal interface IPropertyTypeProvider
    {
        string GetPropertyType(ConfigurationPropertyModelBase propertyModel);
    }

    internal class PropertyTypeProvider : IPropertyTypeProvider
    {
        public string GetPropertyType(ConfigurationPropertyModelBase propertyModel)
        {
            return GetPropertyType((dynamic)propertyModel);
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
                return ConfigurationPropertyType.Enum;
            return ConfigurationPropertyType.Unacceptable;
        }

        private string GetPropertyType(ConfigurationPropertyWithOptionsModelDefinition definition)
        {
            var multipleDefinition = definition as ConfigurationPropertyWithMultipleOptionsModelDefinition;
            if (multipleDefinition != null)
                return ConfigurationPropertyType.MultipleOption;
            return ConfigurationPropertyType.Option;
        }

        private string GetPropertyType(ConfigurationCollectionPropertyDefinition definition)
        {
            return ConfigurationPropertyType.Collection;
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
