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
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyModelBase>();
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
        public Dictionary<string, ConfigurationPropertyModelBase> ConfigurationProperties {get; set;}

        /// <summary>
        /// Gets all properties in config model
        /// </summary>
        /// <returns>All ConfigurationPropertyModel for model</returns>
        public IEnumerable<ConfigurationPropertyModelBase> GetPropertyDefinitions() => ConfigurationProperties.Values;

    }
}
