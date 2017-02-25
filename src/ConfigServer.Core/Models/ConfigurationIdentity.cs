namespace ConfigServer.Core
{
    /// <summary>
    /// Identity of Configuration. 
    /// Includes Client Id
    /// </summary>
    public class ConfigurationIdentity
    {
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
        public string ClientId { get; }

        /// <summary>
        /// Empty Instance
        /// </summary>
        public static readonly ConfigurationIdentity Empty = new ConfigurationIdentity(string.Empty);

        /// <summary>
        /// Determines whether this instance and another specified System.String object have the same value.
        /// </summary>
        /// <param name="obj">The string to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            var identity = obj as ConfigurationIdentity;
            return identity != null && ClientId.Equals(identity.ClientId);
        }

        /// <summary>
        /// Returns the hash code for this string.
        /// </summary>
        /// <returns> A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => ClientId.GetHashCode();
    }
}
