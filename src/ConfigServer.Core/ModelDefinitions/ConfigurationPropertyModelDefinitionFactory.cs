using System;
using System.Reflection;

namespace ConfigServer.Core
{
    internal static class ConfigurationPropertyModelDefinitionFactory
    {
        public static ConfigurationPropertyModelBase Build(PropertyInfo propertyInfo) => Build(propertyInfo.Name, propertyInfo.PropertyType);

        public static ConfigurationPropertyModelBase Build(string propertyName, Type type)
        {
            return new ConfigurationPrimitivePropertyModel(propertyName, type);
        }
    }
}
