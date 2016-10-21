using System;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents a configuration with meta data such as client id
    /// </summary>
    public abstract class ConfigInstance
    {
        /// <summary>
        /// Initializes new ConfigInstance with name
        /// </summary>
        /// <param name="type">type of config</param>
        protected ConfigInstance(Type type)
        {
            Name = type.Name;
            ConfigType = type;
        }

        /// <summary>
        /// Name of configuration
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Client Id for configuration
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets configuration as object
        /// </summary>
        /// <returns>configuration as object</returns>
        public abstract object GetConfiguration();
        
        /// <summary>
        /// Sets configuration
        /// </summary>
        /// <param name="value">value of configuration</param>
        public abstract void SetConfiguration(object value);

        /// <summary>
        /// Constructs a new instance of the configuration
        /// </summary>
        /// <returns>New instance of the configuration </returns>
        public abstract object ConstructNewConfiguration();

        /// <summary>
        /// Type of Configuration
        /// </summary>
        public Type ConfigType { get; }
    }
}
