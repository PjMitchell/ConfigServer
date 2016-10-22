namespace ConfigServer.Server
{
    internal class ConfigurationModelSummary
    {
        /// <summary>
        /// Id defaults to config type
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Display name for configuration
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Description for configuration
        /// </summary>
        public string Description { get; set; }
    }
}
