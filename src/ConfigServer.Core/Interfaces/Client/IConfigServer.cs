using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    /// <summary>
    /// Client for ConfigServer
    /// </summary>
    public interface IConfigServer
    {
        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be build</typeparam>
        /// <returns>Configuration of specified type</returns>
        Task<TConfig> GetConfigAsync<TConfig>() where TConfig : class, new();

        /// <summary>
        /// Gets Configuration for specified client
        /// </summary>
        /// <param name="clientId">client Id</param>
        /// <typeparam name="TConfig">Type of configuration to be build</typeparam>
        /// <returns>Configuration of specified type</returns>
        Task<TConfig> GetConfigAsync<TConfig>(string clientId) where TConfig : class, new();

        /// <summary>
        /// Builds Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be build</param>
        /// <returns>Configuration of specified type</returns>
        Task<object> GetConfigAsync(Type type);

        /// <summary>
        /// Builds Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be build</param>
        /// <param name="clientId">client Id</param>
        /// <returns>Configuration of specified type</returns>
        Task<object> GetConfigAsync(Type type, string clientId);

        /// <summary>
        /// Gets Configuration that is a collection
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be build</typeparam>
        /// <returns>IEnumerable of configuration of specified type</returns>
        Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>() where TConfig : class, new();

        /// <summary>
        /// Gets Configuration that is a collection
        /// </summary>
        /// <param name="clientId">client Id</param>
        /// <typeparam name="TConfig">Type of configuration to be build</typeparam>
        /// <returns>IEnumerable of configuration of specified type</returns>
        Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>(string clientId) where TConfig : class, new();

    }
}
