using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Exception thrown if Client is requested, but not found in registry
    /// </summary>
    public class ConfigurationClientNotFoundException : Exception
    {
        /// <summary>
        /// Constructs ConfigurationClientNotFoundException for missing clientId
        /// </summary>
        /// <param name="clientId">Client Id not found</param>
        public ConfigurationClientNotFoundException(string clientId) : base($"Could not find client with Id:{clientId}")
        {

        }
    }
}
