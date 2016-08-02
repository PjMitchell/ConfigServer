using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ConfigServer.Core.Tests")]
namespace ConfigServer.Core
{
    /// <summary>
    /// Builder for ConfigurationSetModel
    /// </summary>
    public class ConfigurationSetModelBuilder
    {
        private readonly ConfigurationSetModel definition;

        internal ConfigurationSetModelBuilder(Type type, string name, string description)
        {
            definition = new ConfigurationSetModel(type, name, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfig> Config<TConfig>() => Config<TConfig>(typeof(TConfig).Name, string.Empty);

        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <param name="name">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfig> Config<TConfig>(string name) => Config<TConfig>(name, string.Empty);


        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <param name="name">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfig> Config<TConfig>(string name, string description)
        {
            var model = definition.GetOrInitialize<TConfig>();
            model.ConfigurationDisplayName = name;
            model.ConfigurationDescription = description;
            return new ConfigurationModelBuilder<TConfig>(model);
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
