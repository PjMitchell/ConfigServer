using System;

namespace ConfigServer.Core
{
    public sealed class ConfigurationRegistration
    {
        private readonly Func<Config> initializer;

        public static ConfigurationRegistration Build<TConfig>() where TConfig : class, new()
        {
            Func<Config> initializer = () => new Config<TConfig>();
            return new ConfigurationRegistration(typeof(TConfig), initializer);
        }

        private ConfigurationRegistration(Type type, Func<Config> initializer)
        {
            ConfigurationName = type.Name;
            ConfigType = type;
            this.initializer = initializer;
        }

        public string ConfigurationName { get; }
        public ConfigurationModelDefinition ModelDefinition { get; }
        public Type ConfigType { get; }

        public Config InitializeConfig()
        {
            return initializer.Invoke();
        }
    }
}
