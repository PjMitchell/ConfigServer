using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Dynamic;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace ConfigServer.Server
{
    internal interface IConfigurationEditPayloadMapper
    {
        object MapToEditConfig(ConfigInstance config, ConfigurationSetModel model);
        ConfigInstance UpdateConfigurationInstance(ConfigInstance original, JObject newEditPayload, ConfigurationSetModel model);
    }


    internal class ConfigurationEditPayloadMapper : IConfigurationEditPayloadMapper
    {
        readonly IPropertyTypeProvider propertyTypeProvider;
        readonly IServiceProvider serviceProvider;

        public ConfigurationEditPayloadMapper(IServiceProvider serviceProvider, IPropertyTypeProvider propertyTypeProvider)
        {
            this.propertyTypeProvider = propertyTypeProvider;
            this.serviceProvider = serviceProvider;
        }

        public object MapToEditConfig(ConfigInstance config, ConfigurationSetModel model)
        {
            var configModel = model.Configs.Single(s => s.Type == config.ConfigType);
            var source = config.GetConfiguration();
            return BuildObject(source, configModel.ConfigurationProperties);
        }

        public ConfigInstance UpdateConfigurationInstance(ConfigInstance original, JObject newEditPayload, ConfigurationSetModel model)
        {
            var configModel = model.Configs.Single(s => s.Type == original.ConfigType);
            var newConfig = UpdateObject(original.ConstructNewConfiguration(), newEditPayload,configModel.ConfigurationProperties);
            original.SetConfiguration(newConfig);
            return original; 
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
            var value = propertyModel.GetPropertyValue(source);
            if (value == null)
                return null;
            return  propertyModel.GetKeyFromObject(value);
        }

        private object GetPropertyValue(object source, ConfigurationPropertyWithMultipleOptionsModelDefinition propertyModel)
        {
            var collection = propertyModel.GetPropertyValue(source) as IEnumerable ?? new List<object>();
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
            var collection = propertyModel.GetPropertyValue(source) as IEnumerable ?? new List<object>();

            var result = new List<object>();
            foreach (var item in collection)
            {
                var itemValue = BuildObject(item, propertyModel.ConfigurationProperties);
                result.Add(itemValue);
            }

            return result;
        }

        private object UpdateObject(object target, JObject source, Dictionary<string, ConfigurationPropertyModelBase> properties)
        {
            foreach (var property in properties)
            {
                var propertyType = propertyTypeProvider.GetPropertyType(property.Value);
                property.Value.SetPropertyValue(target, GetConfigPropertyValueFromInput(propertyType, source, property.Value));
            }
            return target;
        }

        private object GetConfigPropertyValueFromInput(string propertyType, JObject source, ConfigurationPropertyModelBase propertyModel)
        {
            switch (propertyType)
            {
                case ConfigurationPropertyType.Option:
                    return GetConfigPropertyValueFromInput(source, (ConfigurationPropertyWithOptionsModelDefinition)propertyModel);
                case ConfigurationPropertyType.MultipleOption:
                    return GetConfigPropertyValueFromInput(source, (ConfigurationPropertyWithMultipleOptionsModelDefinition)propertyModel);
                case ConfigurationPropertyType.Collection:
                    return GetConfigPropertyValueFromInput(source, (ConfigurationCollectionPropertyDefinition)propertyModel);
                default:
                    return GetConfigPropertyValueFromInput(source, propertyModel);

            }
        }

        private object GetConfigPropertyValueFromInput(JObject source, ConfigurationPropertyModelBase propertyModel)
        {
            var result = source.GetValue(propertyModel.ConfigurationPropertyName.ToLowerCamelCase()).ToObject(propertyModel.PropertyType);
            return result;
        }

        private object GetConfigPropertyValueFromInput(JObject source, ConfigurationPropertyWithOptionsModelDefinition propertyModel)
        {
            var key = source.GetValue(propertyModel.ConfigurationPropertyName.ToLowerCamelCase()).ToObject<string>();
            object option = null;
            propertyModel.TryGetOption(serviceProvider, key, out option);
            return option;
        }

        private object GetConfigPropertyValueFromInput(JObject source, ConfigurationPropertyWithMultipleOptionsModelDefinition propertyModel)
        {
            var collectionBuilder = propertyModel.GetCollectionBuilder();
            foreach (var key in source.GetValue(propertyModel.ConfigurationPropertyName.ToLowerCamelCase()).Select(s => s.ToObject<string>()))
            {
                object option = null;
                if (propertyModel.TryGetOption(serviceProvider, key, out option))
                    collectionBuilder.Add(option);
            }
            return collectionBuilder.Collection;
        }

        private object GetConfigPropertyValueFromInput(JObject source, ConfigurationCollectionPropertyDefinition propertyModel)
        {
            var collectionBuilder = propertyModel.GetCollectionBuilder();
            foreach (var item in source.GetValue(propertyModel.ConfigurationPropertyName.ToLowerCamelCase()))
            {
                var config = collectionBuilder.IntializeNewItem();
                config = UpdateObject(config, (JObject)item, propertyModel.ConfigurationProperties);
                collectionBuilder.Add(config);
            }
            return collectionBuilder.Collection;
        }
    }
}
