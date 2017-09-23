using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    /// <summary>
    /// Access to Config Archives
    /// </summary>
    public interface IConfigArchive
    {
        /// <summary>
        /// Returns all archived ConfigArchiveEntryInfos for a configuration identity
        /// </summary>
        /// <param name="identity">Configuration identity being queryied</param>
        /// <returns>All archived ConfigArchiveEntryInfo for a configuration identity</returns>
        Task<IEnumerable<ConfigArchiveEntryInfo>> GetArchiveConfigCatalogue(ConfigurationIdentity identity);

        /// <summary>
        /// Gets archived resource for a configuration identity
        /// </summary>
        /// <param name="name">Name of config resource</param>
        /// <param name="identity">Configuration identity being queryied</param>
        /// <returns>Config response</returns>
        Task<ConfigArchiveEntry> GetArchiveConfig(string name, ConfigurationIdentity identity);

        /// <summary>
        /// Deletes archived file for client
        /// </summary>
        /// <param name="name">Name of archived file to be seleted</param>
        /// <param name="identity">Configuration identity being modified</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task DeleteArchiveConfig(string name, ConfigurationIdentity identity);

        /// <summary>
        /// Deletes archived files for client before selected data
        /// </summary>
        /// <param name="deletionDate">Date to delete archives before</param>
        /// <param name="identity">Configuration identity being modified</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task DeleteOldArchiveConfigs(DateTime deletionDate, ConfigurationIdentity identity);
    }
}
