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
        Task SaveChangesAsync(Config config);

        /// <summary>
        /// Get all Client Ids in store
        /// </summary>
        /// <returns>AvailableClientIds</returns>
        Task<IEnumerable<string>> GetClientIdsAsync();

        /// <summary>
        /// Creates new client in store
        /// </summary>
        /// <param name="clientId">new client Id</param>
        /// <returns>A task that represents the asynchronous creation operation.</returns>
        Task CreateClientAsync(string clientId);
    }
}
