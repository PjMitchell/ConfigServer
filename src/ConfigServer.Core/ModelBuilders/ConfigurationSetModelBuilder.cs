using System;

namespace ConfigServer.Core
{
    /// <summary>
    /// Builder for ConfigurationSetModel
    /// </summary>
    public class ConfigurationSetModelBuilder
    {
        private readonly ConfigurationSetModel definition;

        internal ConfigurationSetModelBuilder(Type type)
        {
            definition = new ConfigurationSetModel(type);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfig> Config<TConfig>()
        {
            return new ConfigurationModelBuilder<TConfig>(definition.GetOrInitialize<TConfig>());
        }

        /// <summary>
        /// Adds configuration to ConfigurationSetModel
        /// </summary>
        /// <param name="type">type of configuration to be added to configuration set</param>
        public void AddConfig(Type type)
        {
            definition.GetOrInitialize(type);
        }

        /// <summary>
        /// Returns ConfigurationSetModel setup by builder
        /// </summary>
        /// <returns>ConfigurationSetModel setup by builder</returns>
        public ConfigurationSetModel Build() 
        {
            return definition;
        }
    }
}
