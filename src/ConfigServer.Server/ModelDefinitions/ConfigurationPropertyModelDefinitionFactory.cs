using System;
using System.Reflection;

namespace ConfigServer.Server
{
    internal static class ConfigurationPropertyModelDefinitionFactory
    {
        public static ConfigurationPropertyModelBase Build(PropertyInfo propertyInfo, Type parentType) => Build(propertyInfo.Name, propertyInfo.PropertyType, parentType);

        public static ConfigurationPropertyModelBase Build(string propertyName, Type type, Type parentType)
        {
            return new ConfigurationPrimitivePropertyModel(propertyName, type, parentType);
        }
    }
}
