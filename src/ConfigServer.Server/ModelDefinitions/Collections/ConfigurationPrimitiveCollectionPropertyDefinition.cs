using ConfigServer.Core;
using System;

namespace ConfigServer.Server
{

    internal abstract class ConfigurationPrimitiveCollectionPropertyDefinition : ConfigurationPropertyModelBase
    {

        protected ConfigurationPrimitiveCollectionPropertyDefinition(string propertyName, Type propertyType, Type parentPropertyType, Type collectionType) : base(propertyName, propertyType, parentPropertyType)
        {
            CollectionType = collectionType;
            ValidationRules = new ConfigurationPropertyValidationDefinition();
        }

        /// <summary>
        /// Type of ICollection to be used
        /// </summary>
        public Type CollectionType { get; }

        /// <summary>
        /// Validation rules for property
        /// </summary>
        public ConfigurationPropertyValidationDefinition ValidationRules { get; }


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
