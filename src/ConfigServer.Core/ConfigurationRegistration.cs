using System;

namespace ConfigServer.Core
{
    public sealed class ConfigurationRegistration
    {
        public static ConfigurationRegistration Build<TConfig>() where TConfig : class, new()
        {
            return new ConfigurationRegistration(typeof(TConfig));
        }

        private ConfigurationRegistration(Type type)
        {
            ConfigurationName = type.Name;
            ConfigType = type;
        }

        public string ConfigurationName { get; }
        public Type ConfigType { get; }

    }
}
