using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Core
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
        public ConfigurationPrimitivePropertyModel(string propertyName, Type propertyType) : base(propertyName, propertyType)
        {
            ValidationRules = new ConfigurationPropertyValidationDefinition();
        }

        /// <summary>
        /// Validation rules for property
        /// </summary>
        public ConfigurationPropertyValidationDefinition ValidationRules { get; }
    }
}
