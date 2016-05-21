using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    public class ConfigurationModelDefinition
    {
        public ConfigurationModelDefinition(Type type)
        {
            Type = type;
            ConfigurationDisplayName = type.Name;
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyDefinition>();
        }

        public Type Type { get; }
        public string ConfigurationDisplayName { get; set; }
        public string ConfigurationDescription { get; set; }
        public Dictionary<string, ConfigurationPropertyDefinition> ConfigurationProperties {get; set;}

        public ConfigurationPropertyDefinition GetPropertyDefinition(string propertyName)
        {
            ConfigurationPropertyDefinition result;
            if (!ConfigurationProperties.TryGetValue(propertyName, out result))
                result = new ConfigurationPropertyDefinition(propertyName);
            return result;
        }
    }

    public class ConfigurationPropertyDefinition
    {
        public ConfigurationPropertyDefinition(string propertyName)
        {
            ConfigurationPropertyName = propertyName;
            PropertyDisplayName = propertyName;
        }

        public string ConfigurationPropertyName { get; }
        public string PropertyDisplayName { get; set; }
        public string PropertyDescription { get; set; }

    }
}
