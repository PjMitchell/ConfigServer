namespace ConfigServer.Core
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Used for string values 
    /// </summary>
    public class ConfigurationStringPropertyBuilder : ConfigurationPropertyModelBuilder<ConfigurationStringPropertyBuilder>
    {
        /// <summary>
        /// Initializes ConfigurationStringPropertyBuilder for given ConfigurationPropertyModel
        /// </summary>
        /// <param name="model">ConfigurationPropertyModel to be modified by ConfigurationPropertyModelBuilder</param>
        public ConfigurationStringPropertyBuilder(ConfigurationPrimitivePropertyModel model) : base(model) { }

        /// <summary>
        /// Sets Maximum length validation rule for property
        /// </summary>
        /// <param name="value">Maximum string length</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationStringPropertyBuilder WithMaxLength(int value)
        {
            ((ConfigurationPrimitivePropertyModel)model).ValidationRules.MaxLength = value;
            return this;
        }

        /// <summary>
        /// Sets Regrex pattern validation rule for property
        /// </summary>
        /// <param name="value">Regrex pattern</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationStringPropertyBuilder WithPattern(string value)
        {
            ((ConfigurationPrimitivePropertyModel)model).ValidationRules.Pattern = value;
            return this;
        }
    }
}
