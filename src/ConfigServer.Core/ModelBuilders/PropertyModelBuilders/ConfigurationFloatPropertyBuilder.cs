namespace ConfigServer.Core
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Used for Numeric values such as floats, doubles and decimal 
    /// </summary>
    public class ConfigurationFloatPropertyBuilder : ConfigurationPropertyModelBuilder<ConfigurationFloatPropertyBuilder>
    {
        /// <summary>
        /// Initializes ConfigurationFloatPropertyBuilder for given ConfigurationPropertyModel
        /// </summary>
        /// <param name="model">ConfigurationPropertyModel to be modified by ConfigurationPropertyModelBuilder</param>
        public ConfigurationFloatPropertyBuilder(ConfigurationPrimitivePropertyModel model) : base(model) { }

        /// <summary>
        /// Sets Maximum value validation rule for property
        /// </summary>
        /// <param name="value">Maximum value</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationFloatPropertyBuilder WithMaxValue(double value)
        {
            ((ConfigurationPrimitivePropertyModel)model).ValidationRules.Max = value;
            return this;
        }

        /// <summary>
        /// Sets Minimum value validation rule for property
        /// </summary>
        /// <param name="value">Minimum value</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationFloatPropertyBuilder WithMinValue(double value)
        {
            ((ConfigurationPrimitivePropertyModel)model).ValidationRules.Min = value;
            return this;
        }
    }
}
