using System;
using System.Collections;
using System.Collections.Generic;
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
        /// <typeparam name="TConfiguration">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>ConfigInstance of the type requested</returns>
        Task<ConfigInstance<TConfiguration>> GetAsync<TConfiguration>(ConfigurationIdentity id) where TConfiguration : class, new();

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>ConfigInstance of the type requested</returns>
        Task<ConfigInstance> GetAsync(Type type, ConfigurationIdentity id);

        /// <summary>
        /// Gets Collection Configuration
        /// </summary>
        /// <typeparam name="TConfiguration">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Enumerable of the type requested</returns>
        Task<IEnumerable<TConfiguration>> GetCollectionAsync<TConfiguration>(ConfigurationIdentity id) where TConfiguration : class, new(); 
        /// <summary>
        /// Gets Collection Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Enumerable of the type requested</returns>
        Task<IEnumerable> GetCollectionAsync(Type type, ConfigurationIdentity id);
    }
}
