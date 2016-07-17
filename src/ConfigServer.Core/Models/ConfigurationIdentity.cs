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
        /// ClientId for configuration
        /// </summary>
        public string ClientId { get; set; }
    }
}
