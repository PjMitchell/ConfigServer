using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace ConfigServer.Core
{
    public class ConfigurationCollection : IEnumerable<ConfigurationRegistration>
    {
        private readonly HashSet<ConfigurationRegistration> collection;
        public ConfigurationCollection()
        {
            collection = new HashSet<ConfigurationRegistration>();
        }

        public bool AddRegistration(ConfigurationRegistration registration)
        {
            return collection.Add(registration);
        }

        public IEnumerator<ConfigurationRegistration> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }
    }
}
