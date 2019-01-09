using ConfigServer.Core;
using System;

namespace ConfigServer.Server
{

    internal abstract class ConfigurationPrimitiveCollectionPropertyDefinition : ConfigurationPropertyModelBase
    {

        protected ConfigurationPrimitiveCollectionPropertyDefinition(string propertyName, Type propertyType, Type parentPropertyType, Type collectionType) : base(propertyName, propertyType, parentPropertyType)
        {
            CollectionType = collectionType;
        }

        /// <summary>
        /// Type of ICollection to be used
        /// </summary>
        public Type CollectionType { get; }

        /// <summary>
        /// Indicates If a collection has a unique key
        /// </summary>
        public bool HasUniqueKey { get; set; }


        /// <summary>
        /// Gets Collection Builder for Property
        /// </summary>
        /// <returns>Collection Builder for Property</returns>
        public abstract CollectionBuilder GetCollectionBuilder();
    }

    internal class ConfigurationPrimitiveCollectionPropertyDefinition<TConfig> : ConfigurationPrimitiveCollectionPropertyDefinition
    {

        internal ConfigurationPrimitiveCollectionPropertyDefinition(string propertyName, Type propertyType, Type parentPropertyType, Type collectionType) : base(propertyName, propertyType, parentPropertyType, collectionType)
        {

        }

        /// <summary>
        /// Gets Collection Builder for Property
        /// </summary>
        /// <returns>Collection Builder for Property</returns>
        public override CollectionBuilder GetCollectionBuilder() => new CollectionBuilder<TConfig>(CollectionType);
    }

}
