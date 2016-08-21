using ConfigServer.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Server
{
    internal static class ConfigFormBinder
    {
        public static ConfigInstance BindForm(ConfigInstance existingConfig, HttpContext context, ConfigurationModel model)
        {
            var configItem = existingConfig.GetConfiguration();
            foreach(var prop in model.ConfigurationProperties)
            {
                SetValueFromForm(existingConfig, context, (dynamic)prop.Value);
            }
            return existingConfig;
        }

        private static void SetValueFromForm(ConfigInstance existingConfig, HttpContext context, ConfigurationPrimitivePropertyModel definition)
        {
            var formValue = context.Request.Form[definition.ConfigurationPropertyName].Single();
            object parsedValue;
            if (typeof(Enum).IsAssignableFrom(definition.PropertyType))
                parsedValue = Enum.Parse(definition.PropertyType, formValue);
            else
                parsedValue = Convert.ChangeType(formValue, definition.PropertyType);
            definition.SetPropertyValue(existingConfig.GetConfiguration(), parsedValue);
        }

        private static void SetValueFromForm(ConfigInstance existingConfig, HttpContext context, ConfigurationPropertyWithOptionsModelDefinition definition)
        {
            var formValue = context.Request.Form[definition.ConfigurationPropertyName].Single();
            object parsedValue;
            definition.TryGetOption(context.RequestServices, formValue, out parsedValue);
            definition.SetPropertyValue(existingConfig.GetConfiguration(), parsedValue);
        }

        private static void SetValueFromForm(ConfigInstance existingConfig, HttpContext context, ConfigurationPropertyWithMultipleOptionsModelDefinition definition)
        {
            var formValues = context.Request.Form[definition.ConfigurationPropertyName];
            definition.SetPropertyValue(context.RequestServices, existingConfig.GetConfiguration(), formValues);
        }
    }
}
