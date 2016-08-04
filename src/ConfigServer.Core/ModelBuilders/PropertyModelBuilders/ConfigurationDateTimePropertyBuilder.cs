using System;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Used for Date time values 
    /// </summary>
    public class ConfigurationDateTimePropertyBuilder : ConfigurationPropertyModelBuilder<ConfigurationDateTimePropertyBuilder>
    {
        /// <summary>
        /// Initializes ConfigurationDateTimePropertyBuilder for given ConfigurationPropertyModel
        /// </summary>
        /// <param name="model">ConfigurationPropertyModel to be modified by ConfigurationPropertyModelBuilder</param>
        public ConfigurationDateTimePropertyBuilder(ConfigurationPrimitivePropertyModel model) : base(model) { }

        /// <summary>
        /// Sets Maximum value validation rule for property
        /// </summary>
        /// <param name="value">Maximum value</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationDateTimePropertyBuilder WithMaxValue(DateTime value)
        {
            ((ConfigurationPrimitivePropertyModel)model).ValidationRules.Max = value;
            return this;
        }

        /// <summary>
        /// Sets Minimum value validation rule for property
        /// </summary>
        /// <param name="value">Minimum value</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationDateTimePropertyBuilder WithMinValue(DateTime value)
        {
            ((ConfigurationPrimitivePropertyModel)model).ValidationRules.Min = value;
            return this;
        }
    }
}
