using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration that contains the information required to build, configure and validate the configuration.
    /// </summary>
    public abstract class ConfigurationModel
    {
        /// <summary>
        /// Initialize ConfigurationModel for type
        /// </summary>
        /// <param name="name">Name that identifies config</param>
        /// <param name="type">Type of config</param>
        /// <param name="configurationSetType">ConfigurationSet type that contains configuration</param>
        public ConfigurationModel(string name, Type type, Type configurationSetType)
        {
            Type = type;
            ConfigurationSetType = configurationSetType;
            Name = name;
            ConfigurationDisplayName = type.Name;
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyModelBase>();
        }

        /// <summary>
        /// Configuration type 
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Configuration Set type 
        /// </summary>
        public Type ConfigurationSetType { get; }

        /// <summary>
        /// Name that identifies Config in Config Set
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Display name for configuration
        /// </summary>
        public string ConfigurationDisplayName { get; set; }

        /// <summary>
        /// Description for configuration
        /// </summary>
        public string ConfigurationDescription { get; set; }

        /// <summary>
        /// Property models for configuration
        /// </summary>
        public Dictionary<string, ConfigurationPropertyModelBase> ConfigurationProperties { get; set; }

        /// <summary>
        /// Gets all properties in config model
        /// </summary>
        /// <returns>All ConfigurationPropertyModel for model</returns>
        public IEnumerable<ConfigurationPropertyModelBase> GetPropertyDefinitions() => ConfigurationProperties.Values;

        /// <summary>
        /// Gets Dependencies for Configuration Model
        /// </summary>
        /// <returns>Dependencies for Configuration Model</returns>
        public IEnumerable<ConfigurationDependency> GetDependencies() => ConfigurationProperties.SelectMany(r => r.Value.GetDependencies()).Distinct();

        /// <summary>
        /// Gets Configuration from ConfigurationSet
        /// </summary>
        /// <param name="configurationSet">ConfigurationSet</param>
        /// <returns>Configuration</returns>
        public abstract object GetConfigurationFromConfigurationSet(object configurationSet);

        /// <summary>
        /// Gets Configuration from ConfigurationSet
        /// </summary>
        /// <param name="configurationSet">ConfigurationSet</param>
        /// <param name="value">Config To be added</param>
        /// <returns>Configuration</returns>
        public abstract void SetConfigurationOnConfigurationSet(object configurationSet,object value);

        /// <summary>
        /// Gets Configuration from ConfigurationSet
        /// </summary>
        /// <param name="configurationSet">ConfigurationSet</param>
        /// <returns>Configuration</returns>
        public abstract ConfigInstance GetConfigInstanceFromConfigurationSet(object configurationSet);

        /// <summary>
        /// Is configuration Readonly, generated from a provider external to configuration Server
        /// </summary>
        public bool IsReadOnly { get; protected set; }
    }

    internal class ConfigurationModel<TConfiguration, TConfigurationSet> : ConfigurationModel where TConfigurationSet : ConfigurationSet where TConfiguration : class, new()
    {
        private readonly Func<TConfigurationSet, Config<TConfiguration>> configSelector;
        private readonly Action<TConfigurationSet, Config<TConfiguration>> configSetter;
        public ConfigurationModel(string name, Func<TConfigurationSet, Config<TConfiguration>> configSelector, Action<TConfigurationSet, Config<TConfiguration>> configSetter) : base(name,typeof(TConfiguration), typeof(TConfigurationSet))
        {
            this.configSelector = configSelector;
            this.configSetter = configSetter;
        }

        public override object GetConfigurationFromConfigurationSet(object configurationSet)
        {
            return configSelector((TConfigurationSet)configurationSet).GetConfig();
        }

        public override void SetConfigurationOnConfigurationSet(object configurationSet, object value)
        {
            configSetter((TConfigurationSet)configurationSet,new Config<TConfiguration>((TConfiguration)value));
        }

        public override ConfigInstance GetConfigInstanceFromConfigurationSet(object configurationSet)
        {
            var castConfigurationSet = (TConfigurationSet)configurationSet;
            var config = configSelector(castConfigurationSet).GetConfig();
            return new ConfigInstance<TConfiguration>((TConfiguration)config, castConfigurationSet.Instance.ClientId); 
        }
    }
}
