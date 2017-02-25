using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Used for propeties that are selected from an array of options from a configuration set
    /// </summary>
    public class ConfigurationPropertyWithConfigSetOptionBuilder : ConfigurationPropertyModelBuilder<ConfigurationPropertyWithConfigSetOptionBuilder>
    {
        /// <summary>
        /// Initializes ConfigurationPropertyWithConfigSetOptionBuilder for given ConfigurationPropertyModel
        /// </summary>
        /// <param name="model">ConfigurationPropertyModel to be modified by ConfigurationPropertyModelBuilder</param>
        internal ConfigurationPropertyWithConfigSetOptionBuilder(ConfigurationPropertyWithConfigSetOptionsModelDefinition model) : base(model)
        {
        }
    }
}
