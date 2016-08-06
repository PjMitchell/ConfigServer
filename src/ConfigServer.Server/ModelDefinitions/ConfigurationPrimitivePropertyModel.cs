using System;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Linq;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// </summary>
    public class ConfigurationPrimitivePropertyModel : ConfigurationPropertyModelBase
    {
        /// <summary>
        /// Initialize ConfigurationPrimitivePropertyModel with property name
        /// </summary>
        /// <param name="propertyName">configuration property name</param>
        /// <param name="propertyType">configuration property type</param>
        /// <param name="parentPropertyType">configuration property parent type</param>
        public ConfigurationPrimitivePropertyModel(string propertyName, Type propertyType, Type parentPropertyType) : base(propertyName, propertyType, parentPropertyType)
        {
            ValidationRules = new ConfigurationPropertyValidationDefinition();
        }

        /// <summary>
        /// Validation rules for property
        /// </summary>
        public ConfigurationPropertyValidationDefinition ValidationRules { get; }

    }
}
