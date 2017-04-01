using ConfigServer.Core;
using System;

namespace ConfigServer.Server
{
    internal class ConfigurationUpdatedEvent : IEvent
    {
        public ConfigurationUpdatedEvent(Type configurationType, ConfigurationIdentity identity)
        {
            ConfigurationType = configurationType;
            Identity = identity;
        }

        public ConfigurationUpdatedEvent(ConfigInstance instance) : this(instance.ConfigType, instance.ConfigurationIdentity) { }

        public Type ConfigurationType { get; }
        public ConfigurationIdentity Identity { get; }
    }
}
