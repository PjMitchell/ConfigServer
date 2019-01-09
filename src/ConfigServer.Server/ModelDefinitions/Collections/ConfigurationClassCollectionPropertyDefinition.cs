using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Server
{

    internal abstract class ConfigurationClassCollectionPropertyDefinition : ConfigurationPropertyModelBase
    {

        protected ConfigurationClassCollectionPropertyDefinition(string propertyName, Type propertyType, Type parentPropertyType, Type collectionType) : base(propertyName, propertyType, parentPropertyType)
        {
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyModelBase>();
            CollectionType = collectionType;
        }

        /// <summary>
        /// Property models for configuration
        /// </summary>
        public Dictionary<string, ConfigurationPropertyModelBase> ConfigurationProperties { get; set; }

        /// <summary>
        /// Type of ICollection to be used
        /// </summary>
        public Type CollectionType { get; }

        /// <summary>
        /// Indicates If a collection has a unique key
        /// </summary>
        public bool HasUniqueKey { get; set; }

        /// <summary>
        /// Property of collection model that is used for unique key
        /// </summary>
        public string KeyPropertyName { get; set; }

        /// <summary>
        /// Gets Collection Builder for Property
        /// </summary>
        /// <returns>Collection Builder for Property</returns>
        public abstract CollectionBuilder GetCollectionBuilder();

        public abstract object NewItemInstance();

        /// <summary>
        /// Gets Key From Collection Item
        /// </summary>
        /// <param name="member">Collection Item</param>
        /// <returns>Key for Collection Item</returns>
        public string GetKeyFromMember(object member)
        {
            return PropertyType.GetProperty(KeyPropertyName).GetValue(member).ToString();
        }

        /// <summary>
        /// Gets Dependencies for property
        /// </summary>
        /// <returns>ConfigurationDependency for property</returns>
        public override IEnumerable<ConfigurationDependency> GetDependencies() => ConfigurationProperties.Values.SelectMany(v => v.GetDependencies());
    }

    internal class ConfigurationClassCollectionPropertyDefinition<TConfig> : ConfigurationClassCollectionPropertyDefinition where TConfig : new()
    {

        internal ConfigurationClassCollectionPropertyDefinition(string propertyName, Type propertyType, Type parentPropertyType, Type collectionType) : base(propertyName, propertyType, parentPropertyType, collectionType)
        {
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyModelBase>();
        }

        /// <summary>
        /// Gets Collection Builder for Property
        /// </summary>
        /// <returns>Collection Builder for Property</returns>
        public override CollectionBuilder GetCollectionBuilder() => new CollectionBuilder<TConfig>(CollectionType);

        public override object NewItemInstance() => new TConfig();
    }
}
