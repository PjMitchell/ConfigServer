using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Property Builder for a property that is a collection
    /// </summary>
    public class ConfigurationIntegerCollectionPropertyBuilder<TPropertyType> : ConfigurationPrimitiveCollectionPropertyBuilder where TPropertyType : IComparable
    {
        private readonly ConfigurationPrimitiveCollectionPropertyDefinition definition;

        internal ConfigurationIntegerCollectionPropertyBuilder(ConfigurationPrimitiveCollectionPropertyDefinition definition) : base(definition)
        {
            this.definition = definition;
        }

        /// <summary>
        /// Flags that every member of the collection should be unique
        /// </summary>
        /// <param name="hasUniqueValues">is every member of the collection unique?</param>
        /// <returns>Builder</returns>
        public ConfigurationIntegerCollectionPropertyBuilder<TPropertyType> HasUniqueValues(bool hasUniqueValues = true)
        {
            definition.ValidationRules.AllowDuplicates = !hasUniqueValues;
            return this;
        }

        /// <summary>
        /// Sets Maximum value validation rule for property
        /// </summary>
        /// <param name="value">Maximum value</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationIntegerCollectionPropertyBuilder<TPropertyType> WithMaxValue(TPropertyType value) 
        {
            definition.ValidationRules.Max = value;
            return this;
        }

        /// <summary>
        /// Sets Minimum value validation rule for property
        /// </summary>
        /// <param name="value">Minimum value</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationIntegerCollectionPropertyBuilder<TPropertyType> WithMinValue(TPropertyType value)
        {
            definition.ValidationRules.Min = value;
            return this;
        }
    }
}
