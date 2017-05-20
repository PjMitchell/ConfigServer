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
            return new ConfigurationRegistration(typeof(TConfig), typeof(TConfig).Name, false);
        }

        /// <summary>
        /// Builds new registration for configuration type 
        /// </summary>
        /// <typeparam name="TConfig">type of configuration to be registered</typeparam>
        /// <param name="name">Name of config specified on the config server</param>
        /// <returns>Initialized ConfigurationRegistry for specified type</returns>
        public static ConfigurationRegistration Build<TConfig>(string name) where TConfig : class, new()
        {
            return new ConfigurationRegistration(typeof(TConfig),name, false);
        }

        /// <summary>
        /// Builds new registration for collection configuration type 
        /// </summary>
        /// <typeparam name="TConfig">type of configuration to be registered</typeparam>
        /// <returns>Initialized ConfigurationRegistry for specified type</returns>
        public static ConfigurationRegistration BuildCollection<TConfig>() where TConfig : class, new()
        {
            return new ConfigurationRegistration(typeof(TConfig), typeof(TConfig).Name, true);
        }

        /// <summary>
        /// Builds new registration for collection configuration type 
        /// </summary>
        /// <typeparam name="TConfig">type of configuration to be registered</typeparam>
        /// <param name="name">Name of config specified on the config server</param>
        /// <returns>Initialized ConfigurationRegistry for specified type</returns>
        public static ConfigurationRegistration BuildCollection<TConfig>(string name) where TConfig : class, new()
        {
            return new ConfigurationRegistration(typeof(TConfig), name, true);
        }

        private ConfigurationRegistration(Type type,string name, bool isCollection)
        {
            ConfigurationName = name;
            ConfigType = type;
            IsCollection = isCollection;
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

        /// <summary>
        /// Is Configuration Collection
        /// </summary>
        public bool IsCollection { get; }
    }
}
