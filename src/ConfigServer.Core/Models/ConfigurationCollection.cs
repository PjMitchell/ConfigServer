using System;
using System.Collections.Generic;
using System.Collections;

namespace ConfigServer.Core
{
    public class ConfigurationCollection : IEnumerable<ConfigurationRegistration>
    {
        private readonly Dictionary<Type, ConfigurationRegistration> collection;
        public ConfigurationCollection()
        {
            collection = new Dictionary<Type, ConfigurationRegistration>();
        }

        public bool AddRegistration(ConfigurationRegistration registration)
        {
            if (collection.ContainsKey(registration.ConfigType))
                return false;
            collection.Add(registration.ConfigType, registration);
            return true;
        }

        public bool BuildAndAddRegistration<TConfig>() where TConfig : class, new()
        {
            return AddRegistration(ConfigurationRegistration.Build<TConfig>());
        }

        public ConfigurationRegistration Get(Type type)
        {
            return collection[type];
        }

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
