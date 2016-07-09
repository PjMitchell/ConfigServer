namespace ConfigServer.Core
{
    /// <summary>
    /// Validation information for a configuration property
    /// </summary>
    public class ConfigurationPropertyValidationDefinition
    {
        /// <summary>
        /// Minimum value for a property
        /// </summary>
        public object Min { get; set; }

        /// <summary>
        /// Maximum value for a property
        /// </summary>
        public object Max { get; set; }

        /// <summary>
        /// Maximum length for a property
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Regrex pattern a string property must match
        /// </summary>
        public string Pattern { get; set; }
    }
}
