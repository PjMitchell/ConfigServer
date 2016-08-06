using System;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    /// <summary>
    /// Client for ConfigServer
    /// </summary>
    public interface IConfigServerClient
    {
        /// <summary>
        /// Builds Configuration
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be build</typeparam>
        /// <returns>Configuration of specified type</returns>
        Task<TConfig> BuildConfigAsync<TConfig>() where TConfig : class, new();

        /// <summary>
        /// Builds Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be build</param>
        /// <returns>Configuration of specified type</returns>
        Task<object> BuildConfigAsync(Type type);
    }
}
