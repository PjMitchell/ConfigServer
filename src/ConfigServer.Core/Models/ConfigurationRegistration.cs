using System;

namespace ConfigServer.Core
{
    /// <summary>
    /// Configuration registration used in ConfigurationRegistration
    /// </summary>
    public sealed class ConfigurationRegistration
    {
        /// <summary>
        /// Builds new registration for configuration type 
        /// </summary>
        /// <typeparam name="TConfig">type of configuration to be registered</typeparam>
        /// <returns>Initialized ConfigurationRegistry for specified type</returns>
        public static ConfigurationRegistration Build<TConfig>() where TConfig : class, new()
        {
            return new ConfigurationRegistration(typeof(TConfig));
        }

        private ConfigurationRegistration(Type type)
        {
            ConfigurationName = type.Name;
            ConfigType = type;
        }

        /// <summary>
        /// Name of configuration
        /// Defaults to type Name
        /// </summary>
        public string ConfigurationName { get; }

        /// <summary>
        /// Configuration type
        /// </summary>
        public Type ConfigType { get; }
    }
}
