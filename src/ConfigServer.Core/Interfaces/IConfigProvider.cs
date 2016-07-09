using System;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    /// <summary>
    /// Provides configuration from ConfigServer
    /// </summary>
    public interface IConfigProvider
    {
        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Config of the type requested</returns>
        Task<Config<TConfig>> GetAsync<TConfig>(ConfigurationIdentity id) where TConfig : class, new();

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Config of the type requested</returns>
        Task<Config> GetAsync(Type type, ConfigurationIdentity id);
    }
}
