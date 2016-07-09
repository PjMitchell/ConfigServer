using System;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents errors that occur when a configuration model is not found for a type 
    /// </summary>
    public class ConfigurationModelNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ConfigurationModelNotFoundException class for a given configuration model that was not found. 
        /// </summary>
        /// <param name="type">Type not found with configuration model.</param>
        public ConfigurationModelNotFoundException(Type type) : base($"Could not find Configuration of Type: {type.FullName}") { }
    }
}
