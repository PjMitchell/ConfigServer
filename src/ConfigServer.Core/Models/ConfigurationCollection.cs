using System;
using System.Collections.Generic;
using System.Collections;

namespace ConfigServer.Core
{
    /// <summary>
    /// Registry of configurations in a client
    /// </summary>
    public class ConfigurationRegistry : IEnumerable<ConfigurationRegistration>
    {
        private readonly Dictionary<Type, ConfigurationRegistration> collection;

        /// <summary>
        /// Initializes new ConfigurationRegistry
        /// </summary>
        public ConfigurationRegistry()
        {
            collection = new Dictionary<Type, ConfigurationRegistration>();
        }

        /// <summary>
        /// Adds configuration to registry 
        /// </summary>
        /// <param name="registration">Configuration registration to be added</param>
        /// <returns>returns true if successful or false if registration already in registry</returns>
        public bool AddRegistration(ConfigurationRegistration registration)
        {
            if (collection.ContainsKey(registration.ConfigType))
                return false;
            collection.Add(registration.ConfigType, registration);
            return true;
        }

        /// <summary>
        /// Constructs Registration for config and adds it to the registry
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be added to the registry</typeparam>
        /// <returns>returns true if successful or false if registration already in registry</returns>
        public bool BuildAndAddRegistration<TConfig>() where TConfig : class, new()
        {
            return AddRegistration(ConfigurationRegistration.Build<TConfig>());
        }

        /// <summary>
        /// Gets Registration for type
        /// </summary>
        /// <param name="type">type of configuration expected in the registry</param>
        /// <returns>ConfigurationRegistration for type</returns>
        /// <exception cref="KeyNotFoundException">When type not found in registry</exception>
        public ConfigurationRegistration Get(Type type)
        {
            return collection[type];
        }

        /// <summary>
        /// Tries to get Registration for type
        /// </summary>
        /// <param name="type">Type of config being requested</param>
        /// <param name="value">ConfigurationRegistration for type</param>
        /// <returns>True if found, false if not</returns>
        public bool TryGet(Type type, out ConfigurationRegistration value) => collection.TryGetValue(type, out value);

        /// <summary>
        /// Returns an enumerator that iterates through the ConfigurationRegistrations.
        /// </summary>
        /// <returns>Enumerator that iterates through the ConfigurationRegistrations.</returns>
        public IEnumerator<ConfigurationRegistration> GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }
    }
}
