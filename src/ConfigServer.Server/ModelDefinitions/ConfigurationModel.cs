using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration that contains the information required to build, configure and validate the configuration.
    /// </summary>
    public class ConfigurationModel
    {
        /// <summary>
        /// Initialize ConfigurationModel for type
        /// </summary>
        /// <param name="name">Name that identifies config</param>
        /// <param name="type">Type of config</param>
        public ConfigurationModel(string name,Type type)
        {
            Type = type;
            Name = name;
            ConfigurationDisplayName = type.Name;
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyModelBase>();
        }

        /// <summary>
        /// Configuration type 
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Name that identifies Config in Config Set
        /// </summary>
        public string Name { get; }

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
