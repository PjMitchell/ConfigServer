using System;
using System.Collections.Generic;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents the model of the configuration set that contains the information required to build, configure and validate the configuration.
    /// </summary>
    public class ConfigurationSetModel
    {
        private readonly Dictionary<Type, ConfigurationModel> configurationModels;

        /// <summary>
        /// Initializes configuration set model for configuration set type
        /// </summary>
        /// <param name="configSetType">Configuration set type</param>
        public ConfigurationSetModel(Type configSetType)
        {
            ConfigSetType = configSetType;
            configurationModels = new Dictionary<Type, ConfigurationModel>();
        }

        /// <summary>
        /// Configuration set type
        /// </summary>
        public Type ConfigSetType { get; }

        /// <summary>
        /// Gets or initializes a configuration model by type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type of configuration model to be retrieved or initialized</typeparam>
        /// <returns>Configuration model for type</returns>
        public ConfigurationModel GetOrInitialize<TConfig>() => GetOrInitialize(typeof(TConfig));

        /// <summary>
        /// Gets or initializes a configuration model by type
        /// </summary>
        /// <param name="type">Configuration type of configuration model to be retrieved or initialized</param>
        /// <returns>Configuration model for type</returns>
        public ConfigurationModel GetOrInitialize(Type type)
        {
            ConfigurationModel definition;
            if(!configurationModels.TryGetValue(type, out definition))
            {
                definition = new ConfigurationModel(type);
                configurationModels.Add(type, definition);
            }
            return definition;
        }

        /// <summary>
        /// Gets a configuration model by type
        /// </summary>
        /// <param name="type">Configuration type of configuration model to be retrieved</param>
        /// <returns>Configuration model for type</returns>
        /// <exception cref="ConfigurationModelNotFoundException">Thrown if configuration type not found</exception>
        public ConfigurationModel Get(Type type)
        {
            ConfigurationModel definition;
            if (!configurationModels.TryGetValue(type, out definition))
            {
                throw new ConfigurationModelNotFoundException(type);
            }
            return definition;
        }

        /// <summary>
        /// Gets a configuration model by type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type of configuration model to be retrieved</typeparam>
        /// <returns>Configuration model for type</returns>
        /// <exception cref="ConfigurationModelNotFoundException">Thrown if configuration type not found</exception>
        public ConfigurationModel Get<TConfig>() => Get(typeof(TConfig));

        /// <summary>
        /// Configuration models for configuration set
        /// </summary>
        public IEnumerable<ConfigurationModel> Configs => configurationModels.Values;
    }
}
