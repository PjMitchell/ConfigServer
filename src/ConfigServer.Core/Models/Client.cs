namespace ConfigServer.Core
{
    /// <summary>
    /// Represents a client who has a configuration in ConfigServer
    /// </summary>
    public class ConfigurationClient
    {
        /// <summary>
        /// Identifier for Client
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Display name for client
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of client
        /// </summary>
        public string Description { get; set; }
    }
}
