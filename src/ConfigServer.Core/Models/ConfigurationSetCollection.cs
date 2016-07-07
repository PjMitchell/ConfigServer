using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    public class ConfigurationSetCollection : IEnumerable<ConfigurationSetDefinition>
    {
        private readonly Dictionary<Type, ConfigurationSetDefinition> collection;
        public ConfigurationSetCollection()
        {
            collection = new Dictionary<Type, ConfigurationSetDefinition>();
        }

        public bool AddConfigurationSet(ConfigurationSetDefinition definition)
        {
            if (collection.ContainsKey(definition.ConfigSetType))
                return false;
            collection.Add(definition.ConfigSetType, definition);
            return true;
        }

        public ConfigurationModelDefinition GetConfigDefinition<TConfig>()
        {
            return GetConfigDefinition(typeof(TConfig));
        }

        public ConfigurationModelDefinition GetConfigDefinition(Type type)
        {
            return collection.Values.SelectMany(s => s.Configs).First(s => s.Type == type);
        }
        public IEnumerator<ConfigurationSetDefinition> GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.Values.GetEnumerator();
        }
    }
}
