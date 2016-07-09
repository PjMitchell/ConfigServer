namespace ConfigServer.Core
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// </summary>
    public class ConfigurationPropertyModel
    {
        /// <summary>
        /// Initialize ConfigurationPropertyModel with property name
        /// </summary>
        /// <param name="propertyName">configuration property name</param>
        public ConfigurationPropertyModel(string propertyName)
        {
            ConfigurationPropertyName = propertyName;
            PropertyDisplayName = propertyName;
            ValidationRules = new ConfigurationPropertyValidationDefinition();
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
        /// Validation rules for property
        /// </summary>
        public ConfigurationPropertyValidationDefinition ValidationRules { get; }
    }
}
