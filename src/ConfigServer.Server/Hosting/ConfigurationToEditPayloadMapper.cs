using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Dynamic;
using System.Collections;

namespace ConfigServer.Server
{
    internal interface IConfigurationEditPayloadMapper
    {
        object MapToEditConfig(ConfigInstance config, ConfigurationSetModel model);
    }


    internal class ConfigurationEditPayloadMapper : IConfigurationEditPayloadMapper
    {
        readonly IPropertyTypeProvider propertyTypeProvider;

        public ConfigurationEditPayloadMapper(IPropertyTypeProvider propertyTypeProvider)
        {
            this.propertyTypeProvider = propertyTypeProvider;
        }

        public object MapToEditConfig(ConfigInstance config, ConfigurationSetModel model)
        {
            var configModel = model.Configs.Single(s => s.Type == config.ConfigType);
            var source = config.GetConfiguration();
            return BuildObject(source, configModel.ConfigurationProperties);
        }

        private object BuildObject(object source, Dictionary<string, ConfigurationPropertyModelBase> properties)
        {
            IDictionary<string, object> obj = new ExpandoObject();
            foreach (var property in properties)
            {
                var propertyType = propertyTypeProvider.GetPropertyType(property.Value);
                obj[property.Key] = GetPropertyValue(propertyType, source, property.Value);
            }
            return obj;
        }

        private object GetPropertyValue(string propertyType, object source, ConfigurationPropertyModelBase propertyModel)
        {
            switch (propertyType)
            {
                case ConfigurationPropertyType.Option:
                    return GetPropertyValue(source, (ConfigurationPropertyWithOptionsModelDefinition)propertyModel);
                case ConfigurationPropertyType.MultipleOption:
                    return GetPropertyValue(source, (ConfigurationPropertyWithMultipleOptionsModelDefinition)propertyModel);
                case ConfigurationPropertyType.Collection:
                    return GetPropertyValue(source, (ConfigurationCollectionPropertyDefinition)propertyModel);
                default:
                    return propertyModel.GetPropertyValue(source);                        

            }
        }

        private object GetPropertyValue(object source, ConfigurationPropertyWithOptionsModelDefinition propertyModel)
        {
            return propertyModel.GetKeyFromObject(propertyModel.GetPropertyValue(source));
        }

        private object GetPropertyValue(object source, ConfigurationPropertyWithMultipleOptionsModelDefinition propertyModel)
        {
            var collection = (IEnumerable)propertyModel.GetPropertyValue(source);
            var result = new List<string>();
            foreach(var item in collection)
            {
                var itemValue = propertyModel.GetKeyFromObject(item);
                result.Add(itemValue);
            }

            return result;
        }

        private object GetPropertyValue(object source, ConfigurationCollectionPropertyDefinition propertyModel)
        {
            var collection = (IEnumerable)propertyModel.GetPropertyValue(source);
            
            var result = new List<object>();
            foreach (var item in collection)
            {
                var itemValue = BuildObject(item, propertyModel.ConfigurationProperties);
                result.Add(itemValue);
            }

            return result;
        }
    }
}
