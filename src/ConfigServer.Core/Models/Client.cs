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
        /// Defines the group the the client belongs to. Can be empty
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Enviroment of the client e.g. develop, staging, live
        /// </summary>
        public string Enviroment { get; set; }

        /// <summary>
        /// Description of client
        /// </summary>
        public string Description { get; set; }
    }
}
