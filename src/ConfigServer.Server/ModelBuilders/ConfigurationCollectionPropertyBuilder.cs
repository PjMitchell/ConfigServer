using System.Collections.Generic;

namespace ConfigServer.Server
{
    /// <summary>
    /// Property Builder for a property that is a collection
    /// </summary>
    /// <typeparam name="TConfig">Type of object in collection</typeparam>
    public class ConfigurationCollectionPropertyBuilder<TConfig> : IModelWithProperties<TConfig>
    {
        private readonly ConfigurationCollectionPropertyDefinition definition;

        internal ConfigurationCollectionPropertyBuilder(ConfigurationCollectionPropertyDefinition definition)
        {
            this.definition = definition;
        }

        /// <summary>
        /// Propertes of collection object
        /// </summary>
        public Dictionary<string, ConfigurationPropertyModelBase> ConfigurationProperties => definition.ConfigurationProperties;
    }
}
