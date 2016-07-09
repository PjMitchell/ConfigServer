using ConfigServer.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Configurator
{
    internal static class ConfigFormBinder
    {
        public static Config BindForm(Config existingConfig, IFormCollection collection)
        {
            var configItem = existingConfig.GetConfiguration();
            foreach(var prop in existingConfig.ConfigType.GetProperties().Where(prop => prop.CanWrite))
            {
                var value = collection[prop.Name].Single();
                object parsedValue;
                if (typeof(Enum).IsAssignableFrom(prop.PropertyType))
                    parsedValue = Enum.Parse(prop.PropertyType, value);
                else
                    parsedValue = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(configItem, parsedValue);
            }
            return existingConfig;
        }
    }
}
