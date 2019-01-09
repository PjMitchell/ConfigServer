using System.Collections.Generic;

namespace ConfigServer.Server
{
    /// <summary>
    /// Property Builder for a property that is a collection
    /// </summary>
    /// <typeparam name="TConfig">Type of object in collection</typeparam>
    public class ConfigurationClassPropertyBuilder<TConfig> : ConfigurationPropertyModelBuilder<ConfigurationClassPropertyBuilder<TConfig>>, IModelWithProperties<TConfig>
    {
        private readonly ConfigurationClassPropertyDefinition definition;

        internal ConfigurationClassPropertyBuilder(ConfigurationClassPropertyDefinition definition) : base(definition)
        {
            this.definition = definition;
        }

        /// <summary>
        /// Propertes of collection object
        /// </summary>
        public Dictionary<string, ConfigurationPropertyModelBase> ConfigurationProperties => definition.ConfigurationProperties;
    }
}
