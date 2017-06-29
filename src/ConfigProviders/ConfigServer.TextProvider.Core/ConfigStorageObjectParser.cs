using ConfigServer.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.TextProvider.Core
{
    internal class ConfigStorageObjectHelper
    {
        public static object ParseConfigurationStoredObject(string json, Type type)
        {
            var storageObject = JObject.Parse(json);
            var result = storageObject.GetValue(nameof(ConfigStorageObject.Config)).ToObject(type);
            return result;
        }

        public static ConfigStorageObject BuildStorageObject(ConfigInstance config)
        {
            return new ConfigStorageObject
            {
                ServerVersion = config.ConfigurationIdentity.ServerVersion.ToString(),
                ClientId = config.ConfigurationIdentity.Client.ClientId,
                ConfigName = config.Name,
                TimeStamp = DateTime.UtcNow,
                Config = config.GetConfiguration()
            };
        }
    }
}
