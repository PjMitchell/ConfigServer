namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Used for propeties that are selected from an array of options 
    /// </summary>
    public class ConfigurationPropertyWithOptionBuilder : ConfigurationPropertyModelBuilder<ConfigurationPropertyWithOptionBuilder>
    {
        /// <summary>
        /// Initializes ConfigurationPropertyWithOptionBuilder for given ConfigurationPropertyModel
        /// </summary>
        /// <param name="model">ConfigurationPropertyModel to be modified by ConfigurationPropertyModelBuilder</param>
        internal ConfigurationPropertyWithOptionBuilder(ConfigurationPropertyWithOptionsModelDefinition model) : base(model)
        {
        }
    }

    ///// <summary>
    ///// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    ///// Used for propeties that are selected from an array of options 
    ///// </summary>
    //public class ConfigurationPropertyWithConfigSetOptionBuilder : ConfigurationPropertyModelBuilder<ConfigurationPropertyWithConfigSetOptionBuilder>
    //{
    //    /// <summary>
    //    /// Initializes ConfigurationPropertyWithOptionBuilder for given ConfigurationPropertyModel
    //    /// </summary>
    //    /// <param name="model">ConfigurationPropertyModel to be modified by ConfigurationPropertyModelBuilder</param>
    //    internal ConfigurationPropertyWithConfigSetOptionBuilder(ConfigurationPropertyWithConfigSetOptionsModelDefinition model) : base(model)
    //    {
    //    }
    //}
}
