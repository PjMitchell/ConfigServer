using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static ConfigInstance CreateGenericInstance(Type type, string clientId)
        {
            var config = typeof(ConfigInstance<>);
            Type[] typeArgs = { type };
            var configType = config.MakeGenericType(typeArgs);
            var result = (ConfigInstance)Activator.CreateInstance(configType);
            result.ClientId = clientId;
            return result;
        }
    }
}
