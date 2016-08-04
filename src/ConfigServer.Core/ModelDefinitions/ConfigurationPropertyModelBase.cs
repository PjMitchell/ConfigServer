using System;
using System.Reflection;
namespace ConfigServer.Core
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// </summary>
    public abstract class ConfigurationPropertyModelBase
    {
        /// <summary>
        /// Initialize ConfigurationPropertyModel with property name
        /// </summary>
        /// <param name="propertyName">configuration property name</param>
        /// <param name="propertyType">configuration property type</param>
        protected ConfigurationPropertyModelBase(string propertyName, Type propertyType)
        {
            ConfigurationPropertyName = propertyName;
            PropertyDisplayName = propertyName;
            PropertyType = propertyType;
        }

        /// <summary>
        /// Configuration property name
        /// </summary>
        public string ConfigurationPropertyName { get; }

        /// <summary>
        /// Display name for property
        /// </summary>
        public string PropertyDisplayName { get; set; }

        /// <summary>
        /// Description for property
        /// </summary>
        public string PropertyDescription { get; set; }

        /// <summary>
        /// Property type
        /// </summary>
        public Type PropertyType { get; }

        /// <summary>
        /// Gets property value from configuration model
        /// </summary>
        /// <param name="config">Instance of configuration</param>
        /// <returns>Value of property from instance of configuration</returns>
        public object GetPropertyValue(object config) => PropertyType.GetProperty(ConfigurationPropertyName).GetValue(config);
    }
}
