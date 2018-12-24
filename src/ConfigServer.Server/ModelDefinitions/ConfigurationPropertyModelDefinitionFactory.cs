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
                var configurationPropertyAttribute = GetConfigurationPropertyAttributeOrDefault(property);
                if (configurationPropertyAttribute != null)
                    yield return new KeyValuePair<string, ConfigurationPropertyModelBase>(property.Name, BuildModelFromAttribute(configurationPropertyAttribute,property, model));
                else if (IsPrimitiveProperty(property, typeInfo))
                    yield return new KeyValuePair<string, ConfigurationPropertyModelBase>(property.Name, Build(property, model));
                
            }
        }

        private static ConfigurationPropertyModelBase BuildModelFromAttribute(ConfigurationPropertyAttribute configurationPropertyAttribute, PropertyInfo property, Type model)
        {
            switch(configurationPropertyAttribute)
            {
                case ConfigurationClassAttribute classAttribute:
                    return BuildClassModel(property, model);
                case OptionAttribute optionAttribute:
                    return BuildOptionModel(optionAttribute,property, model);
                case OptionValueAttribute optionValueAttribute:
                    return BuildOptionValueModel(optionValueAttribute, property, model);
                default:
                    throw new InvalidOperationException($"{configurationPropertyAttribute.GetType()} is not a known ConfigurationClassAttribute");
            }
        }

        private static ConfigurationPropertyAttribute GetConfigurationPropertyAttributeOrDefault(PropertyInfo propertyInfo)
        {
            var attribtues = propertyInfo.GetCustomAttributes<ConfigurationPropertyAttribute>()
                .Take(2)
                .ToArray();
            switch(attribtues.Length)
            {
                case 0:
                    return null;
                case 1:
                    return attribtues[0];
                default:
                    throw new InvalidOperationException("Found multiple ConfigurationPropertyAttributes");
            }
        }

        #region Option Property Builder

        private static ConfigurationPropertyModelBase BuildOptionModel(OptionAttribute optionAttribute, PropertyInfo property, Type parentType)
        {
            var optionProvider = GetOptionProvider(optionAttribute.OptionProviderType, property, parentType);
            var optionCollectionType = ReflectionHelpers.BuildGenericType(typeof(ICollection<>), optionProvider.OptionType);
            var isCollection = optionCollectionType.IsAssignableFrom(property.PropertyType);
            if(isCollection)
                return BuildCollectionOptionModel(optionProvider, property, parentType);
            return BuildOptionModel(optionProvider, property, parentType);
        }

        private static ConfigurationPropertyModelBase BuildOptionModel(IConfigurationSetOptionProvider optionProvider, PropertyInfo property, Type parentType)
        {
            var definitionType = ReflectionHelpers.BuildGenericType(typeof(ConfigurationPropertyWithOptionModelDefinition<,>), optionProvider.ConfigurationSetType, optionProvider.OptionType);
            var constructor = definitionType.GetConstructors().Single();
            var propertyModel = (ConfigurationPropertyWithOptionModelDefinition)constructor.Invoke(new object[] { optionProvider, property.Name, parentType });
            return propertyModel;
        }

        private static ConfigurationPropertyModelBase BuildCollectionOptionModel(IConfigurationSetOptionProvider optionProvider, PropertyInfo property, Type parentType)
        {
            var definitionType = ReflectionHelpers.BuildGenericType(typeof(ConfigurationPropertyWithMultipleOptionsModelDefinition<,,>), optionProvider.ConfigurationSetType, optionProvider.OptionType, property.PropertyType);
            var constructor = definitionType.GetConstructors().Single();
            var propertyModel = (ConfigurationPropertyWithOptionModelDefinition)constructor.Invoke(new object[] { optionProvider, property.Name, parentType });
            return propertyModel;
        }

        private static IConfigurationSetOptionProvider GetOptionProvider(Type optionProviderType, PropertyInfo property, Type parentType)
        {
            if (!HasEmptyConstructor(optionProviderType))
                throw new InvalidOperationException($"OptionAttribute for {property.Name} on {parentType} does not have an OptionProviderType({optionProviderType}) with a parameterless constructor");
            var instanceFromProviderFromType = Activator.CreateInstance(optionProviderType);
            if(instanceFromProviderFromType is IConfigurationSetOptionProvider optionProvider)
            {
                var expectedOptionProviderType = ReflectionHelpers.BuildGenericType(typeof(IConfigurationSetOptionProvider<,>), optionProvider.ConfigurationSetType, optionProvider.OptionType);
                if (expectedOptionProviderType.IsInstanceOfType(optionProvider))
                    return optionProvider;
            }
            throw new InvalidOperationException($"OptionAttribute for {property.Name} on {parentType} does not have a valid OptionProviderType({optionProviderType})");
        }

        private static ConfigurationPropertyModelBase BuildOptionValueModel(OptionValueAttribute optionValueAttribute, PropertyInfo property, Type parentType)
        {
            var optionProvider = GetOptionValueProvider(optionValueAttribute.OptionValueProviderType, property, parentType);
            var optionCollectionType = ReflectionHelpers.BuildGenericType(typeof(ICollection<>), optionProvider.OptionValueType);
            var isCollection = optionCollectionType.IsAssignableFrom(property.PropertyType);
            if (isCollection)
                return BuildCollectionOptionValueModel(optionProvider, property, parentType);
            return BuildOptionValueModel(optionProvider, property, parentType);
        }

        private static ConfigurationPropertyWithOptionValueModelDefinition BuildOptionValueModel(IConfigurationSetOptionValueProvider optionProvider, PropertyInfo property, Type parentType)
        {
            var definitionType = ReflectionHelpers.BuildGenericType(typeof(ConfigurationPropertyWithOptionValueModelDefinition<,,>), optionProvider.ConfigurationSetType, optionProvider.OptionType, optionProvider.OptionValueType);
            var constructor = definitionType.GetConstructors().Single();
            var propertyModel = (ConfigurationPropertyWithOptionValueModelDefinition)constructor.Invoke(new object[] { optionProvider, property.Name, parentType });
            return propertyModel;
        }

        private static ConfigurationPropertyWithMultipleOptionValuesModelDefinition BuildCollectionOptionValueModel(IConfigurationSetOptionValueProvider optionProvider, PropertyInfo property, Type parentType)
        {
            var definitionType = ReflectionHelpers.BuildGenericType(typeof(ConfigurationPropertyWithMultipleOptionValuesModelDefinition<,,,>), optionProvider.ConfigurationSetType, optionProvider.OptionType, optionProvider.OptionValueType, property.PropertyType);
            var constructor = definitionType.GetConstructors().Single();
            var propertyModel = (ConfigurationPropertyWithMultipleOptionValuesModelDefinition)constructor.Invoke(new object[] { optionProvider, property.Name, parentType });
            return propertyModel;
        }

        private static IConfigurationSetOptionValueProvider GetOptionValueProvider(Type optionProviderType, PropertyInfo property, Type parentType)
        {
            if (!HasEmptyConstructor(optionProviderType))
                throw new InvalidOperationException($"OptionValueAttribute for {property.Name} on {parentType} does not have an OptionProviderType({optionProviderType}) with a parameterless constructor");
            var instanceFromProviderFromType = Activator.CreateInstance(optionProviderType);
            if (instanceFromProviderFromType is IConfigurationSetOptionValueProvider optionProvider)
            {
                var expectedOptionProviderType = ReflectionHelpers.BuildGenericType(typeof(IConfigurationSetOptionValueProvider<,,>), optionProvider.ConfigurationSetType, optionProvider.OptionType, optionProvider.OptionValueType);
                if (expectedOptionProviderType.IsInstanceOfType(optionProvider))
                    return optionProvider;
            }
            throw new InvalidOperationException($"OptionValueAttribute for {property.Name} on {parentType} does not have a valid OptionProviderType({optionProviderType})");
        }

        #endregion

        private static ConfigurationPropertyModelBase BuildClassModel(PropertyInfo property, Type parentType)
        {
            if (!HasEmptyConstructor(property.PropertyType))
                throw new InvalidOperationException($"Nested Class property {property.Name} on {parentType} does not have a parameterless constructor");
            var definitionType = ReflectionHelpers.BuildGenericType(typeof(ConfigurationClassPropertyDefinition<>), property.PropertyType);
            var constructor = definitionType.GetConstructors().Single();
            var propertyModel = (ConfigurationClassPropertyDefinition)constructor.Invoke(new object[] { property.Name, property.PropertyType, parentType });
            propertyModel.ConfigurationProperties = GetDefaultConfigProperties(propertyModel.PropertyType).ToDictionary(k => k.Key, v => v.Value);
            return propertyModel;
        }


        private static bool HasEmptyConstructor(Type propertyType)
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
