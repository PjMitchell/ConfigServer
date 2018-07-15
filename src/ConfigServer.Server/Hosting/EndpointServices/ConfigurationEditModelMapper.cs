using ConfigServer.Core;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Collections;

namespace ConfigServer.Server
{
    internal interface IConfigurationEditModelMapper
    {
        object MapToEditConfig(ConfigInstance config, ConfigurationModel configModel);
    }

    internal class ConfigurationEditModelMapper : IConfigurationEditModelMapper
    {
        readonly IPropertyTypeProvider propertyTypeProvider;
        readonly IOptionSetFactory optionSetFactory;
        readonly IConfigurationSetService configurationSetService;

        public ConfigurationEditModelMapper(IOptionSetFactory optionSetFactory, IPropertyTypeProvider propertyTypeProvider, IConfigurationSetService configurationSetService)
        {
            this.propertyTypeProvider = propertyTypeProvider;
            this.optionSetFactory = optionSetFactory;
            this.configurationSetService = configurationSetService;
        }

        public object MapToEditConfig(ConfigInstance config, ConfigurationModel configModel)
        {
            var source = config.GetConfiguration();
            return BuildObject(source, configModel);
        }

        private object BuildObject(object source, ConfigurationModel model)
        {
            if (model is ConfigurationOptionModel optionModel)
                return BuildObject(source, optionModel);
            return BuildObject(source, model.ConfigurationProperties);
        }

        private object BuildObject(object source, ConfigurationOptionModel model)
        {
            var collection = source as IEnumerable ?? new List<object>();

            var result = new List<object>();
            foreach (var item in collection)
            {
                var itemValue = BuildObject(item, model.ConfigurationProperties);
                result.Add(itemValue);
            }

            return result;
        }

        private object BuildObject(object source, Dictionary<string, ConfigurationPropertyModelBase> properties)
        {
            IDictionary<string, object> obj = new ExpandoObject();
            foreach (var property in properties)
            {
                var propertyType = propertyTypeProvider.GetPropertyType(property.Value);
                obj[property.Key] = GetPropertyValueFromConfig(propertyType, source, property.Value);
            }
            return obj;
        }

        private object GetPropertyValueFromConfig(string propertyType, object source, ConfigurationPropertyModelBase propertyModel)
        {
            switch (propertyType)
            {
                case ConfigurationPropertyType.Option:
                    return GetOptionPropertyValue(source, (IOptionPropertyDefinition)propertyModel);
                case ConfigurationPropertyType.MultipleOption:
                    return GetPropertyValue(source, (IMultipleOptionPropertyDefinition)propertyModel);
                case ConfigurationPropertyType.Collection:
                    return GetPropertyValue(source, (ConfigurationCollectionPropertyDefinition)propertyModel);
                case ConfigurationPropertyType.Class:
                    return GetPropertyValue(source, (ConfigurationClassPropertyDefinition)propertyModel);
                default:
                    return propertyModel.GetPropertyValue(source); 
            }
        }

        private object GetOptionPropertyValue(object source, IOptionPropertyDefinition propertyModel)
        {
            var value = propertyModel.GetPropertyValue(source);
            if (value == null)
                return null;
            if (propertyModel is ConfigurationPropertyWithOptionValueModelDefinition)
                return value;
            return optionSetFactory.GetKeyFromObject(value, propertyModel);
        }

        private object GetPropertyValue(object source, IMultipleOptionPropertyDefinition propertyModel)
        {
            var collection = propertyModel.GetPropertyValue(source) as IEnumerable ?? new List<object>();
            var isOptionValue = propertyModel is ConfigurationPropertyWithMultipleOptionValuesModelDefinition;
            var result = new List<string>();
            foreach(var item in collection)
            {
                var itemValue = isOptionValue? item.ToString() : optionSetFactory.GetKeyFromObject(item, propertyModel);
                result.Add(itemValue);
            }
            return result;
        }

        private object GetPropertyValue(object source, ConfigurationCollectionPropertyDefinition propertyModel)
        {
            var collection = propertyModel.GetPropertyValue(source) as IEnumerable ?? new List<object>();

            var result = new List<object>();
            foreach (var item in collection)
            {
                var itemValue = BuildObject(item, propertyModel.ConfigurationProperties);
                result.Add(itemValue);
            }

            return result;
        }

        private object GetPropertyValue(object source, ConfigurationClassPropertyDefinition propertyModel)
        {
            var nestedProperty = propertyModel.GetPropertyValue(source);
            if (nestedProperty == null)
                nestedProperty = propertyModel.NewItemInstance();
            var itemValue = BuildObject(nestedProperty, propertyModel.ConfigurationProperties);
            return itemValue;
        }
    }
}
