using ConfigServer.Core;
using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Configurator
{
    public static class ConfigFormBinder
    {
        public static Config BindForm(Config existingConfig, IFormCollection collection)
        {
            var configItem = existingConfig.GetConfiguration();
            foreach(var prop in existingConfig.ConfigType.GetProperties().Where(prop => prop.CanWrite))
            {
                var val = Convert.ChangeType(collection[prop.Name].Single(), prop.PropertyType);
                prop.SetValue(configItem, val);
            }
            return existingConfig;
        }
    }
}
