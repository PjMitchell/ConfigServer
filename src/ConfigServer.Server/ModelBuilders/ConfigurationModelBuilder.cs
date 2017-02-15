using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    /// <summary>
    /// Builder for ConfigurationModel 
    /// </summary>
    /// <typeparam name="TConfiguration">Configuration type being built</typeparam>
    /// <typeparam name="TConfigurationSet">Configuration Set of type being built</typeparam>
    public class ConfigurationModelBuilder<TConfiguration, TConfigurationSet> : IModelWithProperties<TConfiguration> where TConfigurationSet : ConfigurationSet
    {
        private readonly ConfigurationModel definition;

        internal ConfigurationModelBuilder(ConfigurationModel definition)
        {
            this.definition = definition;
        }

        internal ConfigurationModelBuilder(string name, Func<TConfigurationSet, Config<TConfiguration>> configSelector) : this(new ConfigurationModel<TConfiguration, TConfigurationSet>(name, configSelector)) { }

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
