using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Configuration Property is a collection
    /// </summary>
    public abstract class ConfigurationCollectionPropertyDefinition : ConfigurationPropertyModelBase
    {

        /// <summary>
        /// Initialize ConfigurationCollectionPropertyDefinition
        /// </summary>
        /// <param name="propertyName">configuration property name</param>
        /// <param name="propertyType">configuration property type</param>
        /// <param name="parentPropertyType">configuration property parent type</param>
        /// <param name="collectionType">collection type to be used by config will default to list if ICollection property</param>
        protected ConfigurationCollectionPropertyDefinition(string propertyName, Type propertyType, Type parentPropertyType, Type collectionType) : base(propertyName, propertyType, parentPropertyType)
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

        /// <summary>
        /// Gets Key From Collection Item
        /// </summary>
        /// <param name="member">Collection Item</param>
        /// <returns>Key for Collection Item</returns>
        public string GetKeyFromMember(object member)
        {
            return PropertyType.GetProperty(KeyPropertyName).GetValue(member).ToString();
        }

        
    }

    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Configuration Property is a collection
    /// </summary>
    public class ConfigurationCollectionPropertyDefinition<TConfig> : ConfigurationCollectionPropertyDefinition where TConfig : new()
    {
        /// <summary>
        /// Initialize ConfigurationCollectionPropertyDefinition
        /// </summary>
        /// <param name="propertyName">configuration property name</param>
        /// <param name="propertyType">configuration property type</param>
        /// <param name="parentPropertyType">configuration property parent type</param>
        /// <param name="collectionType">collection type to be used by config will default to list if ICollection property</param>
        public ConfigurationCollectionPropertyDefinition(string propertyName, Type propertyType, Type parentPropertyType, Type collectionType) : base(propertyName, propertyType, parentPropertyType, collectionType)
        {
            ConfigurationProperties = new Dictionary<string, ConfigurationPropertyModelBase>();
        }

        /// <summary>
        /// Gets Collection Builder for Property
        /// </summary>
        /// <returns>Collection Builder for Property</returns>
        public override CollectionBuilder GetCollectionBuilder() => new CollectionBuilder<TConfig>(CollectionType);
    }
}
