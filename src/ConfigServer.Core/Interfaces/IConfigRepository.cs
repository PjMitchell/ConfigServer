using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    /// <summary>
    /// Store for ConfigServer Configurations
    /// </summary>
    public interface IConfigRepository : IConfigProvider
    {
        /// <summary>
        /// Saves changes to configuration
        /// </summary>
        /// <param name="config">Updated configuration to be saved</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task UpdateConfigAsync(Config config);

        /// <summary>
        /// Get all Client in store
        /// </summary>
        /// <returns>Available Client</returns>
        Task<IEnumerable<ConfigurationClient>> GetClientsAsync();

        /// <summary>
        /// Creates or updates client details in store
        /// </summary>
        /// <param name="client">Updated Client detsils</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task UpdateClientAsync(ConfigurationClient client);
    }
}
