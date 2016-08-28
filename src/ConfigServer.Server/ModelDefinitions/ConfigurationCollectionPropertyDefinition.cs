using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Configuration Property is a collection
    /// </summary>
    public class ConfigurationCollectionPropertyDefinition : ConfigurationPropertyModelBase
    {

        /// <summary>
        /// Initialize ConfigurationCollectionPropertyDefinition
        /// </summary>
        /// <param name="propertyName">configuration property name</param>
        /// <param name="propertyType">configuration property type</param>
        /// <param name="parentPropertyType">configuration property parent type</param>
        public ConfigurationCollectionPropertyDefinition(string propertyName, Type propertyType, Type parentPropertyType) : base(propertyName, propertyType, parentPropertyType)
        {
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyModelBase>();
        }

        /// <summary>
        /// Property models for configuration
        /// </summary>
        public Dictionary<string, ConfigurationPropertyModelBase> ConfigurationProperties { get; set; }
    }
}
