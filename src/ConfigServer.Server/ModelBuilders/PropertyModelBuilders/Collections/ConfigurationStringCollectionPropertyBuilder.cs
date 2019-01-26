namespace ConfigServer.Server
{
    /// <summary>
    /// Property Builder for a property that is a collection
    /// </summary>
    public class ConfigurationStringCollectionPropertyBuilder : ConfigurationPrimitiveCollectionPropertyBuilder
    {
        private readonly ConfigurationPrimitiveCollectionPropertyDefinition definition;

        internal ConfigurationStringCollectionPropertyBuilder(ConfigurationPrimitiveCollectionPropertyDefinition definition) : base(definition)
        {
            this.definition = definition;
        }

        /// <summary>
        /// Flags that every member of the collection should be unique
        /// </summary>
        /// <param name="hasUniqueValues">is every member of the collection unique?</param>
        /// <returns>Builder</returns>
        public ConfigurationStringCollectionPropertyBuilder HasUniqueValues(bool hasUniqueValues = true)
        {
            definition.ValidationRules.AllowDuplicates = hasUniqueValues;
            return this;
        }

        /// <summary>
        /// Sets Maximum length validation rule for property
        /// </summary>
        /// <param name="value">Maximum string length</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationStringCollectionPropertyBuilder WithMaxLength(int value)
        {
            definition.ValidationRules.MaxLength = value;
            return this;
        }

        /// <summary>
        /// Sets Regrex pattern validation rule for property
        /// </summary>
        /// <param name="value">Regrex pattern</param>
        /// <returns>ConfigurationPropertyModelBuilder for ConfigurationPropertyModel</returns>
        public ConfigurationStringCollectionPropertyBuilder WithPattern(string value)
        {
            definition.ValidationRules.Pattern = value;
            return this;
        }
    }
}
