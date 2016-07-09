using System;
using System.Collections.Generic;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents the model of the configuration that contains the information required to build, configure and validate the configuration.
    /// </summary>
    public class ConfigurationModel
    {
        /// <summary>
        /// Initialize ConfigurationModel for type
        /// </summary>
        /// <param name="type"></param>
        public ConfigurationModel(Type type)
        {
            Type = type;
            ConfigurationDisplayName = type.Name;
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyModel>();
        }

        /// <summary>
        /// Configuration type 
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Display name for configuration
        /// </summary>
        public string ConfigurationDisplayName { get; set; }

        /// <summary>
        /// Description for configuration
        /// </summary>
        public string ConfigurationDescription { get; set; }

        /// <summary>
        /// Property models for configuration
        /// </summary>
        public Dictionary<string, ConfigurationPropertyModel> ConfigurationProperties {get; set;}

        /// <summary>
        /// Gets or initialize  property model for property name
        /// </summary>
        /// <param name="propertyName">Configuration property name</param>
        /// <returns>ConfigurationPropertyModel for property name</returns>
        public ConfigurationPropertyModel GetPropertyDefinition(string propertyName)
        {
            ConfigurationPropertyModel result;
            if (!ConfigurationProperties.TryGetValue(propertyName, out result))
                result = new ConfigurationPropertyModel(propertyName);
            return result;
        }
    }
}
