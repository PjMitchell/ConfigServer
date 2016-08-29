using ConfigServer.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ConfigServer.Server
{
    internal interface IConfigurationFormBinder
    {
        ConfigInstance BuildConfigurationFromForm(ConfigInstance existingConfig, IFormCollection formCollection, ConfigurationModel model);
        ConfigurationClient BuildConfigurationClientFromForm(IFormCollection collection);
    }

    internal class ConfigurationFormBinder : IConfigurationFormBinder
    {
        private readonly IServiceProvider serviceProvider;

        public ConfigurationFormBinder(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ConfigInstance BuildConfigurationFromForm(ConfigInstance existingConfig, IFormCollection formCollection, ConfigurationModel model)
        {
            var configItem = existingConfig.GetConfiguration();
            foreach(var prop in model.ConfigurationProperties)
            {
                SetValueFromForm(configItem, formCollection, (dynamic)prop.Value);
            }
            return existingConfig;
        }

        public ConfigurationClient BuildConfigurationClientFromForm(IFormCollection collection)
        {
            var result = new ConfigurationClient
            {
                ClientId = collection["ClientId"].Single(),
                Name = collection["Name"].Single(),
                Description = collection["Description"].Single()
            };
            return result;
        }

        private void SetValueFromForm(object existingConfig, IFormCollection formCollection, ConfigurationPrimitivePropertyModel definition)
        {
            var formValue = formCollection[definition.ConfigurationPropertyName].Single();
            object parsedValue;
            if (typeof(Enum).IsAssignableFrom(definition.PropertyType))
                parsedValue = Enum.Parse(definition.PropertyType, formValue);
            else
                parsedValue = Convert.ChangeType(formValue, definition.PropertyType);
            definition.SetPropertyValue(existingConfig, parsedValue);
        }

        private void SetValueFromForm(object existingConfig, IFormCollection formCollection, ConfigurationPropertyWithOptionsModelDefinition definition)
        {
            var formValue = formCollection[definition.ConfigurationPropertyName].Single();
            object parsedValue;
            definition.TryGetOption(serviceProvider, formValue, out parsedValue);
            definition.SetPropertyValue(existingConfig, parsedValue);
        }

        private void SetValueFromForm(object existingConfig, IFormCollection formCollection, ConfigurationPropertyWithMultipleOptionsModelDefinition definition)
        {
            var formValues = formCollection[definition.ConfigurationPropertyName];
            var collectionBuilder = definition.GetCollectionBuilder();
            foreach(var value in formValues)
            {
                object option;
                if (definition.TryGetOption(serviceProvider, value, out option))
                    collectionBuilder.Add(option);
            }
            definition.SetPropertyValue(existingConfig, collectionBuilder.Collection);
        }



        private void SetValueFromForm(object existingConfig, IFormCollection formCollection, ConfigurationCollectionPropertyDefinition definition)
        {

            var match = $"^{definition.ConfigurationPropertyName}";
            var formValues = formCollection.Where(w => Regex.IsMatch(w.Key, match))
                .Select(s => new CollectionKey(s.Key, s.Value));
            //definition.SetPropertyValue(serviceProvider, existingConfig.GetConfiguration(), formValues);
            var collectionBuilder = definition.GetCollectionBuilder();
            foreach (var value in GetItemForForms(formValues))
            {
                var item = collectionBuilder.IntializeNewItem();
                foreach (var prop in definition.ConfigurationProperties)
                {
                    SetValueFromForm(item, value, (dynamic)prop.Value);
                }
                collectionBuilder.Add(item);
            }
            definition.SetPropertyValue(existingConfig, collectionBuilder.Collection);
        }

        private IEnumerable<IFormCollection> GetItemForForms(IEnumerable<CollectionKey> keys)
        {
            var list = new List<PartialForm>();
            foreach (var item in keys)
            {
                while(list.Count < item.Values.Count)
                {
                    list.Add(new PartialForm());
                }
                for (var i = 0; i < item.Values.Count; i++)
                {
                    list[i][item.CollectionProperty] = item.Values[i];
                }
            }
            return list;
        }

        private class CollectionKey
        {
            public CollectionKey(string key, StringValues values)
            {
                var indexOfSeparator = key.IndexOf('.');
                var postSeparatorIndex = indexOfSeparator + 1;
                CollectionProperty = key.Substring(postSeparatorIndex, key.Length - postSeparatorIndex);
                Values = values;
            }
            public string CollectionProperty { get; set; }
            public StringValues Values { get; set; }
        }
    }
}
