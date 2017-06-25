using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.TextProvider.Core
{
    internal class ConfigStorageObjectParser
    {
        public static object ParseConfigurationStoredObject(string json, Type type)
        {
            var storageObject = JObject.Parse(json);
            var result = storageObject.GetValue(nameof(ConfigStorageObject.Config)).ToObject(type);
            return result;
        }
    }
}
