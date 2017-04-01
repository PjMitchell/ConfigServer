using System;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents a client who has a configuration in ConfigServer
    /// </summary>
    public class ConfigurationClient
    {
        /// <summary>
        /// Default constructor for Configuration Client
        /// </summary>
        public ConfigurationClient() : this(string.Empty)
        {

        }

        /// <summary>
        /// Consturction with ClientId param
        /// </summary>
        /// <param name="clientId">Client Id</param>
        public ConfigurationClient(string clientId)
        {
            ClientId = clientId;
        }
        
        
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

        /// <summary>
        /// Determines whether this instance and another specified object have the same value.
        /// </summary>
        /// <param name="obj">The string to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            var client = obj as ConfigurationClient;
            return client != null && StringComparer.OrdinalIgnoreCase.Equals(ClientId, client.ClientId);
        }

        /// <summary>
        /// Returns the hash code for this string.
        /// </summary>
        /// <returns> A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => ClientId == null? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(ClientId);
    }
}
