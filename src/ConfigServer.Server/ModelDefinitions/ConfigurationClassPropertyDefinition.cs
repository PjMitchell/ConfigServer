using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Server
{

    internal abstract class ConfigurationClassPropertyDefinition : ConfigurationPropertyModelBase
    {

        protected ConfigurationClassPropertyDefinition(string propertyName, Type propertyType, Type parentPropertyType) : base(propertyName, propertyType, parentPropertyType)
        {
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyModelBase>();
        }

        /// <summary>
        /// Property models for configuration
        /// </summary>
        public Dictionary<string, ConfigurationPropertyModelBase> ConfigurationProperties { get; set; }

        /// <summary>
        /// Gets Dependencies for property
        /// </summary>
        /// <returns>ConfigurationDependency for property</returns>
        public override IEnumerable<ConfigurationDependency> GetDependencies() => ConfigurationProperties.Values.SelectMany(v => v.GetDependencies());

        public abstract object NewItemInstance();
    }

    internal class ConfigurationClassPropertyDefinition<TConfig> : ConfigurationClassPropertyDefinition where TConfig : new()
    {

        public ConfigurationClassPropertyDefinition(string propertyName, Type propertyType, Type parentPropertyType) : base(propertyName, propertyType, parentPropertyType)
        {
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyModelBase>();
        }

        public override object NewItemInstance() => new TConfig();
    }
}
