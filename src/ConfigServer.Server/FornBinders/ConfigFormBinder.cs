using ConfigServer.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Reflection;

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
                SetValueFromForm(existingConfig, formCollection, (dynamic)prop.Value);
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

        private void SetValueFromForm(ConfigInstance existingConfig, IFormCollection formCollection, ConfigurationPrimitivePropertyModel definition)
        {
            var formValue = formCollection[definition.ConfigurationPropertyName].Single();
            object parsedValue;
            if (typeof(Enum).IsAssignableFrom(definition.PropertyType))
                parsedValue = Enum.Parse(definition.PropertyType, formValue);
            else
                parsedValue = Convert.ChangeType(formValue, definition.PropertyType);
            definition.SetPropertyValue(existingConfig.GetConfiguration(), parsedValue);
        }

        private void SetValueFromForm(ConfigInstance existingConfig, IFormCollection formCollection, ConfigurationPropertyWithOptionsModelDefinition definition)
        {
            var formValue = formCollection[definition.ConfigurationPropertyName].Single();
            object parsedValue;
            definition.TryGetOption(serviceProvider, formValue, out parsedValue);
            definition.SetPropertyValue(existingConfig.GetConfiguration(), parsedValue);
        }

        private void SetValueFromForm(ConfigInstance existingConfig, IFormCollection formCollection, ConfigurationPropertyWithMultipleOptionsModelDefinition definition)
        {
            var formValues = formCollection[definition.ConfigurationPropertyName];
            definition.SetPropertyValue(serviceProvider, existingConfig.GetConfiguration(), formValues);
        }
    }
}
