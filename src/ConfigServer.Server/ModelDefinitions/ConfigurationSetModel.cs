using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration set that contains the information required to build, configure and validate the configuration.
    /// </summary>
    public abstract class ConfigurationSetModel
    {
        protected readonly Dictionary<Type, ConfigurationModel> configurationModels;

        /// <summary>
        /// Initializes configuration set model for configuration set type
        /// </summary>
        /// <param name="configSetType">Configuration set type</param>
        /// <param name="name">Display name for configuration set</param>
        /// <param name="description">Description for configuration set type</param>
        protected ConfigurationSetModel(Type configSetType, string name, string description)
        {
            ConfigSetType = configSetType;
            Name = name;
            Description = description;
            configurationModels = new Dictionary<Type, ConfigurationModel>();
        }

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

        /// <summary>
        /// Gets Dependencies for Configuration Set Model
        /// </summary>
        /// <returns>Dependencies for Configuration Set Model</returns>
        public IEnumerable<ConfigurationDependency> GetDependencies() => configurationModels.SelectMany(r => r.Value.GetDependencies()).Distinct();

        /// <summary>
        /// Does ConfigurationSetModel contain Configuration 
        /// </summary>
        /// <param name="type">Configuration type</param>
        /// <returns>True if ConfigurationSet contains Configuration type</returns>
        public bool ContainsConfig(Type type) => configurationModels.ContainsKey(type);

        protected void ApplyDefaultPropertyDefinitions(ConfigurationModel model)
        {
            foreach (PropertyInfo writeProperty in model.Type.GetProperties().Where(prop => prop.CanWrite))
            {
                model.ConfigurationProperties.Add(writeProperty.Name, ConfigurationPropertyModelDefinitionFactory.Build(writeProperty, model.Type));
            }
        }
    }

    internal class ConfigurationSetModel<TConfigurationSet> : ConfigurationSetModel where TConfigurationSet : ConfigurationSet
    {
        public ConfigurationSetModel(string name, string description) : base(typeof(TConfigurationSet), name, description)
        {
            
        }
        public ConfigurationSetModel() : this(typeof(TConfigurationSet).Name, string.Empty) { }

        /// <summary>
        /// Gets or initializes a configuration option model by type
        /// </summary>
        /// <typeparam name="TOption">Configuration type of configuration model to be retrieved or initialized</typeparam>
        /// <returns>Configuration model for type</returns>
        public ConfigurationModel GetOrInitializeOption<TOption>(string name, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector)
        {
            var type = typeof(TOption);
            ConfigurationModel definition;
            if (!configurationModels.TryGetValue(type, out definition))
            {
                definition = new ConfigurationOptionModel<TOption, TConfigurationSet>(name, keySelector, descriptionSelector);
                ApplyDefaultPropertyDefinitions(definition);
                configurationModels.Add(type, definition);
            }
            if (definition.Name != name)
                throw new InvalidOperationException($"Tried to Get model:{name} of type:{type} when there is already a named config for that type ({definition.Name})");
            if (!(definition is ConfigurationOptionModel<TOption, TConfigurationSet>))
                throw new InvalidOperationException($"Tried to Get model:{name} of type:{type} when model is not setup as an option");
            return definition;
        }

        /// <summary>
        /// Gets or initializes a configuration model by type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type of configuration model to be retrieved or initialized</typeparam>
        /// <returns>Configuration model for type</returns>
        public ConfigurationModel GetOrInitialize<TConfig>(string name, Func<TConfigurationSet, Config<TConfig>> selector)
        {
            var type = typeof(TConfig);
            ConfigurationModel definition;
            if (!configurationModels.TryGetValue(type, out definition))
            {
                definition = new ConfigurationModel<TConfig,TConfigurationSet>(name, selector);
                ApplyDefaultPropertyDefinitions(definition);
                configurationModels.Add(type, definition);
            }
            if (definition.Name != name)
                throw new InvalidOperationException($"Tried to Get model:{name} of type:{type} when there is already a named config for that type ({definition.Name})");
            return definition;
        }

        /// <summary>
        /// Gets or initializes a configuration model by type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type of configuration model to be retrieved or initialized</typeparam>
        /// <returns>Configuration model for type</returns>
        public ConfigurationModel GetOrInitialize(PropertyInfo configProperty)
        {
            var name = configProperty.Name;
            var configType = configProperty.PropertyType.GenericTypeArguments[0];
            ConfigurationModel definition;
            if (!configurationModels.TryGetValue(configType, out definition))
            {
                var configModelType = ReflectionHelpers.BuildGenericType(typeof(ConfigurationModel<,>), configType, typeof(TConfigurationSet));
                var funcType = ReflectionHelpers.BuildGenericType(typeof(Func<,>), typeof(TConfigurationSet), configProperty.PropertyType);
                var selector = configProperty.GetMethod.CreateDelegate(funcType);
                definition = (ConfigurationModel)Activator.CreateInstance(configModelType, name, selector);
                ApplyDefaultPropertyDefinitions(definition);
                configurationModels.Add(configType, definition);
            }
            if (definition.Name != name)
                throw new InvalidOperationException($"Tried to Get model:{name} of type:{configType} when there is already a named config for that type ({definition.Name})");
            return definition;
        }
    }
}
