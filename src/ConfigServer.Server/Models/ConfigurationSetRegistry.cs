using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Server
{
    /// <summary>
    /// Registry of ConfigurationSets
    /// </summary>
    internal class ConfigurationSetRegistry : IConfigurationSetRegistry
    {
        private readonly Dictionary<Type, ConfigurationSetModel> collection;
        private Version version;

        /// <summary>
        /// Initializes new ConfigurationSetRegistry
        /// </summary>
        public ConfigurationSetRegistry()
        {
            collection = new Dictionary<Type, ConfigurationSetModel>();
            version = new Version(0, 0);
        }

        public void SetVersion(Version value) => version = value;

        /// <summary>
        /// Gets version of configuration
        /// </summary>
        /// <returns>Version of configuration</returns>
        public Version GetVersion() => version;

        /// <summary>
        /// Adds new configuration set to the registry
        /// </summary>
        /// <param name="model">ConfigurationSetModel to be added to the registry</param>
        /// <returns>returns true if successful or false if registry already contains configuration set type</returns>
        public bool AddConfigurationSet(ConfigurationSetModel model)
        {
            if (collection.ContainsKey(model.ConfigSetType))
                return false;
            collection.Add(model.ConfigSetType, model);
            return true;
        }

        /// <summary>
        /// Gets definition for configuration type
        /// </summary>
        /// <typeparam name="TConfig">configuration type to be retrieved</typeparam>
        /// <returns>ConfigurationModel for selected configuration type</returns>
        /// <exception cref="InvalidOperationException">throws if multiple or no configuration of specified type have been registered</exception>
        public ConfigurationModel GetConfigDefinition<TConfig>()
        {
            return GetConfigDefinition(typeof(TConfig));
        }

        /// <summary>
        /// Gets definition for configuration type
        /// </summary>
        /// <param name="type">configuration type to be retrieved</param>
        /// <returns>ConfigurationModel for selected configuration type</returns>
        /// <exception cref="InvalidOperationException">throws if multiple or no configuration of specified type have been registered</exception>
        public ConfigurationModel GetConfigDefinition(Type type)
        {
            return collection.Values.SelectMany(s => s.Configs).Single(s => s.Type == type);
        }

        /// <summary>
        /// Gets definition for configuration set type
        /// </summary>
        /// <param name="type">configuration set type to be retrieved</param>
        /// <returns>ConfigurationModel for selected configuration set type</returns>
        public ConfigurationSetModel GetConfigSetDefinition(Type type)
        {
            return collection[type];
        }

        /// <summary>
        /// Tries to get definition for configuration set type
        /// </summary>
        /// <param name="type">configuration set type to be retrieved</param>
        /// <param name="result">ConfigurationModel for selected configuration set type</param>
        /// <returns>True if found else false</returns>
        public bool TryGetConfigSetDefinition(Type type, out ConfigurationSetModel result) => collection.TryGetValue(type, out result);

        /// <summary>
        /// Gets ConfigurationSet for config type
        /// </summary>
        /// <param name="type">configuration type to be queried</param>
        /// <returns>ConfigurationSetModel for Configuration type</returns>
        public ConfigurationSetModel GetConfigSetForConfig(Type type)
        {
            return collection.Values.Single(s=> s.ContainsConfig(type));
        }

        /// <summary>
        /// Returns an enumerator that iterates through the ConfigurationSetModels
        /// </summary>
        /// <returns>Enumerator for the ConfigurationSetCollection</returns>
        public IEnumerator<ConfigurationSetModel> GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }
    }
}
