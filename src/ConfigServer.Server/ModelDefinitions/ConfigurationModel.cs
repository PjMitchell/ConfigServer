using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration that contains the information required to build, configure and validate the configuration.
    /// </summary>
    public class ConfigurationModel
    {
        /// <summary>
        /// Initialize ConfigurationModel for type
        /// </summary>
        /// <param name="name">Name that identifies config</param>
        /// <param name="type">Type of config</param>
        public ConfigurationModel(string name, Type type)
        {
            Type = type;
            Name = name;
            ConfigurationDisplayName = type.Name;
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyModelBase>();
        }

        /// <summary>
        /// Configuration type 
        /// </summary>
        public Type Type { get; }

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

    }

    internal abstract class ConfigurationOptionModel : ConfigurationModel
    {
        public ConfigurationOptionModel(string name, Type type) :base(name, type)
        {
        }

        public abstract IOptionSet BuildOptionSet(IEnumerable souce);
        public Type StoredType { get; protected set; }
        public IEnumerable<ConfigurationDependency> GetDependencies() => ConfigurationProperties.SelectMany(r => r.Value.GetDependencies()).Distinct();
    }

    internal class ConfigurationOptionModel<TOption> : ConfigurationOptionModel
    {
        private readonly Func<TOption, string> keySelector;
        private readonly Func<TOption, object> descriptionSelector;

        public ConfigurationOptionModel(string name, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector) : base(name, typeof(TOption))
        {
            this.keySelector = keySelector;
            this.descriptionSelector = descriptionSelector;
            StoredType = typeof(List<TOption>);
        }
        
        private string DescriptionSelector(TOption option)
        {
            return descriptionSelector(option).ToString();
        }

        public override IOptionSet BuildOptionSet(IEnumerable source)
        {
            return new OptionSet<TOption>(source, keySelector, DescriptionSelector);
        }
    }
}
