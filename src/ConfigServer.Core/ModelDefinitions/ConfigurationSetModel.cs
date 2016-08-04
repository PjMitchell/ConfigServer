using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        /// <param name="name">Display name for configuration set</param>
        /// <param name="description">Description for configuration set type</param>
        public ConfigurationSetModel(Type configSetType, string name, string description)
        {
            ConfigSetType = configSetType;
            Name = name;
            Description = description;
            configurationModels = new Dictionary<Type, ConfigurationModel>();
        }

        /// <summary>
        /// Initializes configuration set model for configuration set type
        /// </summary>
        /// <param name="configSetType">Configuration set type</param>
        public ConfigurationSetModel(Type configSetType) : this(configSetType, configSetType.Name, string.Empty) { }

        /// <summary>
        /// Configuration set type
        /// </summary>
        public Type ConfigSetType { get; }

        /// <summary>
        /// Display name for ConfigInstance set
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// ConfigInstance set description
        /// </summary>
        public string Description { get; }

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
                ApplyDefaultPropertyDefinitions(definition);
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

        private void ApplyDefaultPropertyDefinitions(ConfigurationModel model)
        {
            foreach(PropertyInfo writeProperty in model.Type.GetProperties().Where(prop => prop.CanWrite))
            {
                model.ConfigurationProperties.Add(writeProperty.Name, ConfigurationPropertyModelDefinitionFactory.Build(writeProperty));
            }
        }
    }
}
