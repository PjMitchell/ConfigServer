using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Used for Numeric values such as floats, doubles and decimal 
    /// </summary>
    public class ConfigurationFloatPropertyBuilder<TProperty> : ConfigurationPropertyModelBuilder<ConfigurationFloatPropertyBuilder<TProperty>> where TProperty : IComparable
    {
        /// <summary>
        /// Initializes ConfigurationFloatPropertyBuilder for given ConfigurationPropertyModel
        /// </summary>
        /// <param name="model">ConfigurationPropertyModel to be modified by ConfigurationPropertyModelBuilder</param>
        internal ConfigurationFloatPropertyBuilder(ConfigurationPrimitivePropertyModel model) : base(model) { }

        /// <summary>
        /// Sets Maximum value validation rule for property
        /// </summary>
        /// <param name="value">Maximum value</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationFloatPropertyBuilder<TProperty> WithMaxValue(TProperty value)
        {
            ((ConfigurationPrimitivePropertyModel)model).ValidationRules.Max = value;
            return this;
        }

        /// <summary>
        /// Sets Minimum value validation rule for property
        /// </summary>
        /// <param name="value">Minimum value</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationFloatPropertyBuilder<TProperty> WithMinValue(TProperty value)
        {
            ((ConfigurationPrimitivePropertyModel)model).ValidationRules.Min = value;
            return this;
        }
    }
}
