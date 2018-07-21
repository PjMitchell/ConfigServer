using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Server
{
    internal static class ConfigurationPropertyModelDefinitionFactory
    {
        public static IEnumerable<KeyValuePair<string, ConfigurationPropertyModelBase>> GetDefaultConfigProperties(Type model)
        {
            foreach (PropertyInfo property in model.GetProperties())
            {
                var typeInfo = property.PropertyType.GetTypeInfo();

                if (!property.CanWrite)
                    continue;
                if(IsPrimitiveProperty(property, typeInfo))
                    yield return new KeyValuePair<string, ConfigurationPropertyModelBase>(property.Name, Build(property, model));
                else if(property.HasAttribute<ConfigurationClassAttribute>())
                    yield return new KeyValuePair<string, ConfigurationPropertyModelBase>(property.Name, BuildClassModel(property, model));
            }
        }

        private static ConfigurationPropertyModelBase BuildClassModel(PropertyInfo property, Type parentType)
        {
            if (!IsValidClassProperty(property.PropertyType))
                throw new InvalidOperationException($"Nested Class property {property.Name} on {parentType} does not have a parameterless constructor");
            var definitionType = ReflectionHelpers.BuildGenericType(typeof(ConfigurationClassPropertyDefinition<>), property.PropertyType);
            var constructor = definitionType.GetConstructors().Single();
            var propertyModel = (ConfigurationClassPropertyDefinition)constructor.Invoke(new object[] { property.Name, property.PropertyType, parentType });
            propertyModel.ConfigurationProperties = GetDefaultConfigProperties(propertyModel.PropertyType).ToDictionary(k => k.Key, v => v.Value);
            return propertyModel;
        }

        private static bool IsValidClassProperty(Type propertyType)
        {
            return propertyType.GetConstructor(new Type[0]) != null;
        }

        public static ConfigurationPropertyModelBase Build(PropertyInfo propertyInfo, Type parentType) => Build(propertyInfo.Name, propertyInfo.PropertyType, parentType);

        public static ConfigurationPropertyModelBase Build(string propertyName, Type type, Type parentType)
        {
            var propertyModel = new ConfigurationPrimitivePropertyModel(propertyName, type, parentType);
            var typeInfo = type.GetTypeInfo();
            propertyModel.ValidationRules.IsRequired = !(typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));

            return propertyModel;
        }

        private static bool IsPrimitiveProperty(PropertyInfo info, TypeInfo typeInfo)
        {
            return (typeInfo.IsPrimitive 
                || info.PropertyType == typeof(string) 
                || info.PropertyType == typeof(DateTime)
                || typeInfo.IsEnum
                || IsNullablePrimitive(typeInfo));
        }

        private static bool IsNullablePrimitive(TypeInfo info)
        {
            return info.IsGenericType 
                && info.GetGenericTypeDefinition() == typeof(Nullable<>) 
                && info.GenericTypeArguments.Single().GetTypeInfo().IsPrimitive;
        }
    }
}
