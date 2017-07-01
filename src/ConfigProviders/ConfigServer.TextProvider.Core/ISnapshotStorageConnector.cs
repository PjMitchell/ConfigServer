using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.TextProvider.Core
{
    /// <summary>
    /// Storage Connector for Configuration Snapshot
    /// </summary>
    public interface ISnapshotStorageConnector
    {
        /// <summary>
        /// Gets Snapshot Registry File from storage
        /// </summary>
        /// <returns>Snapshot registry File from storage</returns>
        Task<string> GetSnapshotRegistryFileAsync();

        /// <summary>
        /// Set Snapshot Registry File in storage
        /// </summary>
        /// <param name="value">new value</param>
        /// <returns>Task from operation</returns>
        Task SetSnapshotRegistryFileAsync(string value);

        /// <summary>
        /// Gets Snapshot Entries by Id
        /// </summary>
        /// <param name="snapshotId">snapshot id</param>
        /// <returns>Snapshot Entries for Id</returns>
        Task<IEnumerable<SnapshotTextEntry>> GetSnapshotEntries(string snapshotId);

        /// <summary>
        /// Sets Snapshot Entries for an id
        /// </summary>
        /// <param name="snapshotId">Snapshot id for Entries</param>
        /// <param name="entries">Entries for Id</param>
        /// <returns>Task from operation</returns>
        Task SetSnapshotEntries(string snapshotId, IEnumerable<SnapshotTextEntry> entries);

        /// <summary>
        /// Deletes Snapshot
        /// </summary>
        /// <param name="snapshotId">snapshot id</param>
        /// <returns>Task from operation</returns>
        Task DeleteSnapshot(string snapshotId);
    }
}
