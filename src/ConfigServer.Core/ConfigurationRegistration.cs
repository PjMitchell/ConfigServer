using System;

namespace ConfigServer.Core
{
    public sealed class ConfigurationRegistration
    {
        public ConfigurationRegistration(Type type) :this(type, type.Name) { }

        public ConfigurationRegistration(Type type, string configurationName)
        {
            ConfigurationName = configurationName;
            ConfigType = type;
        }


        public string ConfigurationName { get; }
        public Type ConfigType { get; }
        public override bool Equals(object obj)
        {
            var reg = obj as ConfigurationRegistration;
            return reg.ConfigurationName.Equals(ConfigurationName);
        }

        public override int GetHashCode()
        {
            return ConfigurationName.GetHashCode();
        }
    }
}
