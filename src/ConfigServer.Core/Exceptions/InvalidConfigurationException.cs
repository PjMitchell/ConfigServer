using System;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents errors that occur when a configuration is requested and its type is not in the config registry
    /// </summary>
    public class InvalidConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the InvalidConfigurationException class for a given type that was not found in the config registry. 
        /// </summary>
        /// <param name="type">Type not found in the config registry. </param>
        public InvalidConfigurationException(Type type) : base($"Could not find configuration of type {type.Name}") { }
    }
}
