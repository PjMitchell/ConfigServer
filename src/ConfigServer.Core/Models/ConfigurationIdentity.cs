using System;

namespace ConfigServer.Core
{
    /// <summary>
    /// Identity of Configuration. 
    /// Includes Client Id
    /// </summary>
    public class ConfigurationIdentity
    {
        /// <summary>
        /// Initialize new ConfigurationIdentity
        /// </summary>
        public ConfigurationIdentity(){}

        /// <summary>
        /// Initialize new ConfigurationIdentity with client id
        /// </summary>
        public ConfigurationIdentity(string clientId)
        {
            ClientId = clientId;
        }

        /// <summary>
        /// ClientId for configuration
        /// </summary>
        public string ClientId { get; set; }
    }
}
