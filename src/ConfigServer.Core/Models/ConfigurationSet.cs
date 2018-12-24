
using ConfigServer.Core;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents a set of configurations and sets up the information required to build, configure and validate the configurations.
    /// </summary>
    public abstract class ConfigurationSet
    {
        /// <summary>
        /// Display name for configuartion set
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Description for configuartion set
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Instance of the configuration
        /// </summary>
        public ConfigurationIdentity Instance { get; set; }

    }
}
