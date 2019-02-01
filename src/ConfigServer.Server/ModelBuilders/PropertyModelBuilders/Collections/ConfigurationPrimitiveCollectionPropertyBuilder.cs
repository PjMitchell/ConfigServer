using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Property Builder for a property that is a collection
    /// </summary>
    public class ConfigurationPrimitiveCollectionPropertyBuilder : ConfigurationPropertyModelBuilder<ConfigurationPrimitiveCollectionPropertyBuilder>
    {
        private readonly ConfigurationPrimitiveCollectionPropertyDefinition definition;

        internal ConfigurationPrimitiveCollectionPropertyBuilder(ConfigurationPrimitiveCollectionPropertyDefinition definition) : base(definition)
        {
            this.definition = definition;
        }

        /// <summary>
        /// Flags that every member of the collection should be unique
        /// </summary>
        /// <param name="hasUniqueValues">is every member of the collection unique?</param>
        /// <returns>Builder</returns>
        public ConfigurationPrimitiveCollectionPropertyBuilder HasUniqueValues(bool hasUniqueValues = true)
        {
            definition.ValidationRules.AllowDuplicates = !hasUniqueValues;
            return this;
        }
    }
}
