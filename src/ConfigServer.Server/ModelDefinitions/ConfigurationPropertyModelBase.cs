using Microsoft.AspNetCore.Http;
using System;
using System.Reflection;
namespace ConfigServer.Server
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
        /// <param name="parentPropertyType">configuration property parent type</param>
        protected ConfigurationPropertyModelBase(string propertyName, Type propertyType, Type parentPropertyType)
        {
            ConfigurationPropertyName = propertyName;
            PropertyDisplayName = propertyName;
            PropertyType = propertyType;
            ParentPropertyType = parentPropertyType;
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
        /// Type of Property's Parent
        /// </summary>
        public Type ParentPropertyType { get; }

        /// <summary>
        /// Gets property value from configuration model
        /// </summary>
        /// <param name="config">Instance of configuration</param>
        /// <returns>Value of property from instance of configuration</returns>
        public object GetPropertyValue(object config) => ParentPropertyType.GetProperty(ConfigurationPropertyName).GetValue(config);

        /// <summary>
        /// Sets property value from configuration model
        /// </summary>
        /// <param name="config">Instance of configuration</param>
        /// <param name="value">Inserted valus</param>
        public void SetPropertyValue(object config, object value) => ParentPropertyType.GetProperty(ConfigurationPropertyName).SetValue(config, value);
       
    }
}
