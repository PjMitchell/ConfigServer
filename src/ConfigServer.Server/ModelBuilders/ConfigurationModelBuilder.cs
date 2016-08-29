using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Builder for ConfigurationModel 
    /// </summary>
    /// <typeparam name="TConfig">Configuration type being built</typeparam>
    public class ConfigurationModelBuilder<TConfig> : IModelWithProperties<TConfig>
    {
        private readonly ConfigurationModel definition;

        internal ConfigurationModelBuilder(ConfigurationModel definition)
        {
            this.definition = definition;
        }

        /// <summary>
        /// Configuration Model Builder Model
        /// </summary>
        public Type ModelType => definition.Type;

        /// <summary>
        /// Configuration Model Builder Property Collection
        /// </summary>
        public Dictionary<string, ConfigurationPropertyModelBase> ConfigurationProperties => definition.ConfigurationProperties;


        /// <summary>
        /// Returns ConfigurationModel setup by builder
        /// </summary>
        /// <returns>ConfigurationModel setup by builder</returns>
        public ConfigurationModel Build() => definition;


    }
}
