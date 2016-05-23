using System;

namespace ConfigServer.Core
{
    public class ConfigurationSetBuilder
    {
        private readonly ConfigurationSetDefinition definition;

        private ConfigurationSetBuilder(Type type)
        {
            definition = new ConfigurationSetDefinition(type);
        }

        public static ConfigurationSetBuilder Create<TConfigSet>() where TConfigSet: ConfigurationSet
        {
            return Create(typeof(TConfigSet));
        }

        public static ConfigurationSetBuilder Create(Type type)
        {
            return new ConfigurationSetBuilder(type);
        }

        public ConfigurationModelBuilder<TConfig> Config<TConfig>()
        {
            return new ConfigurationModelBuilder<TConfig>(definition.GetOrInitialize<TConfig>());
        }

        public void AddConfig(Type type)
        {
            definition.GetOrInitialize(type);
        }

        public ConfigurationSetDefinition Build() 
        {
            return definition;
        }
    }
}
