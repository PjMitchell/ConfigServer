using System;
using System.Collections;
using System.Linq;

namespace ConfigServer.Core
{
    /// <summary>
    /// Factory for creating Configs
    /// </summary>
    public static class ConfigFactory
    {
        /// <summary>
        /// Creates Instance of a generic ConfigInstance
        /// </summary>
        /// <param name="type">Type for generic ConfigInstance</param>
        /// <param name="client">Configuration Client</param>
        /// <returns></returns>
        public static ConfigInstance CreateGenericInstance(Type type, ConfigurationClient client)
        {
            var config = typeof(ConfigInstance<>);
            Type[] typeArgs = { type };
            var configType = config.MakeGenericType(typeArgs);
            var result = (ConfigInstance)Activator.CreateInstance(configType);
            result.ConfigurationIdentity = new ConfigurationIdentity(client);
            return result;
        }

        /// <summary>
        /// Creates Instance of a generic collection ConfigInstance
        /// </summary>
        /// <param name="type">Type for generic ConfigInstance</param>
        /// <param name="client">Configuration Client</param>
        /// <returns></returns>
        public static ConfigInstance CreateGenericCollectionInstance(Type type, ConfigurationClient client)
        {
            var config = typeof(ConfigCollectionInstance<>);
            Type[] typeArgs = { type };
            var configType = config.MakeGenericType(typeArgs);
            var result = (ConfigInstance)Activator.CreateInstance(configType);
            result.ConfigurationIdentity = new ConfigurationIdentity(client);
            return result;
        }
    }
}
