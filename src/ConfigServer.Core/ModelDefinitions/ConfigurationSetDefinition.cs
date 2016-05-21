using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Core
{
    public class ConfigurationSetDefinition
    {
        private readonly Dictionary<Type, ConfigurationModelDefinition> configurationModels;

        public ConfigurationSetDefinition(Type configSetType)
        {
            ConfigSetType = configSetType;
            configurationModels = new Dictionary<Type, ConfigurationModelDefinition>();
        }

        public Type ConfigSetType { get; }

        public ConfigurationModelDefinition GetOrInitialize<TConfig>() => GetOrInitialize(typeof(TConfig));

        public ConfigurationModelDefinition GetOrInitialize(Type type)
        {
            ConfigurationModelDefinition definition;
            if(!configurationModels.TryGetValue(type, out definition))
            {
                definition = new ConfigurationModelDefinition(type);
                configurationModels.Add(type, definition);
            }
            return definition;
        }

        public ConfigurationModelDefinition Get(Type type)
        {
            ConfigurationModelDefinition definition;
            if (!configurationModels.TryGetValue(type, out definition))
            {
                throw new ConfigurationModelNotFoundException(type);
            }
            return definition;
        }

        public ConfigurationModelDefinition Get<TConfig>() => Get(typeof(TConfig));

        public IEnumerable<ConfigurationModelDefinition> Configs => configurationModels.Values;



    }
}
