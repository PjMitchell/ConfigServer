namespace ConfigServer.Core
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Used for Enum values 
    /// </summary>
    public class ConfigurationEnumPropertyBuilder : ConfigurationPropertyModelBuilder<ConfigurationEnumPropertyBuilder>
    {
        /// <summary>
        /// Initializes ConfigurationEnumPropertyBuilder for given ConfigurationPropertyModel
        /// </summary>
        /// <param name="model">ConfigurationPropertyModel to be modified by ConfigurationPropertyModelBuilder</param>
        public ConfigurationEnumPropertyBuilder(ConfigurationPropertyModel model) : base(model) { }

    }
}
