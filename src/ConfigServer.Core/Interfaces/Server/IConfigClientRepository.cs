using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    /// <summary>
    /// Store for ConfigServer Configuration Client
    /// </summary>
    public interface IConfigClientRepository
    {
        /// <summary>
        /// Get all Client in store
        /// </summary>
        /// <returns>Available Client</returns>
        Task<IEnumerable<ConfigurationClient>> GetClientsAsync();

        /// <summary>
        /// Creates or updates client details in store
        /// </summary>
        /// <param name="client">Updated Client details</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task UpdateClientAsync(ConfigurationClient client);

        /// <summary>
        /// Get all Client Groups in store
        /// </summary>
        /// <returns>Available Client Groups</returns>
        Task<IEnumerable<ConfigurationClientGroup>> GetClientGroupsAsync();

        /// <summary>
        /// Creates or updates client group details in store
        /// </summary>
        /// <param name="clientGroup">Updated Client Group details</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task UpdateClientGroupAsync(ConfigurationClientGroup clientGroup);

        
    }
}
