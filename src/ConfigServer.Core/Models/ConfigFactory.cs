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
        /// Creates Instance of a generic Config
        /// </summary>
        /// <param name="type">Type for generic Config</param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static Config CreateGenericInstance(Type type, string clientId = "")
        {
            var config = typeof(Config<>);
            Type[] typeArgs = { type };
            var configType = config.MakeGenericType(typeArgs);
            var result = (Config)Activator.CreateInstance(configType);
            result.ClientId = clientId;
            return result;
        }
    }
}
