namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Used for Boolean values 
    /// </summary>
    public class ConfigurationBoolPropertyBuilder : ConfigurationPropertyModelBuilder<ConfigurationBoolPropertyBuilder>
    {
        internal ConfigurationBoolPropertyBuilder(ConfigurationPrimitivePropertyModel model) : base(model) { }
    }
}
