namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Used for Enum values 
    /// </summary>
    public class ConfigurationEnumPropertyBuilder : ConfigurationPropertyModelBuilder<ConfigurationEnumPropertyBuilder>
    {
        internal ConfigurationEnumPropertyBuilder(ConfigurationPrimitivePropertyModel model) : base(model) { }
    }
}
