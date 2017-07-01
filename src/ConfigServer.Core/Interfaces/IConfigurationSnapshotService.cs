using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    /// <summary>
    /// Service for Configurations Snapshot to be implemented by the ClientProvider
    /// </summary>
    public interface IConfigurationSnapshotService
    {
        /// <summary>
        /// Get all snapshots
        /// </summary>
        /// <returns>Summary of all snapshots</returns>
        Task<IEnumerable<SnapshotEntryInfo>> GetSnapshots();

        /// <summary>
        /// Get snapshot by id
        /// </summary>
        /// <param name="snapshotId">snapshot id</param>
        /// <param name="targetConfigurationIdentity">snapshot id</param>
        /// <returns>Snapshot entry for Id</returns>
        Task<ConfigurationSnapshotEntry> GetSnapshot(string snapshotId, ConfigurationIdentity targetConfigurationIdentity);

        /// <summary>
        /// Save snapshot
        /// </summary>
        /// <param name="snapshot">snapshot to saved</param>
        /// <returns>Task for operation</returns>
        Task SaveSnapshot(ConfigurationSnapshotEntry snapshot);

        /// <summary>
        /// Deletes snapshot
        /// </summary>
        /// <param name="snapshotId">snapshot id</param>
        /// <returns>Task for operation</returns>
        Task DeleteSnapshot(string snapshotId);

    }
}
