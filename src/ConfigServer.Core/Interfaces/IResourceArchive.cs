using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    /// <summary>
    /// Access to Resource Archives
    /// </summary>
    public interface IResourceArchive
    {
        /// <summary>
        /// Returns all archived ResourceEntryInfos for a configuration identity
        /// </summary>
        /// <param name="identity">Configuration identity being queryied</param>
        /// <returns>All archived ResourceEntryInfos for a configuration identity</returns>
        Task<IEnumerable<ResourceEntryInfo>> GetArchiveResourceCatalogue(ConfigurationIdentity identity);

        /// <summary>
        /// Gets archived resource for a configuration identity
        /// </summary>
        /// <param name="name">Name of archived resource</param>
        /// <param name="identity">Configuration identity being queryied</param>
        /// <returns>Resource response</returns>
        Task<ResourceEntry> GetArchiveResource(string name, ConfigurationIdentity identity);

        /// <summary>
        /// Deletes archived file for client
        /// </summary>
        /// <param name="name">Name of archived file to be seleted</param>
        /// <param name="identity">Configuration identity being modified</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task DeleteArchiveResource(string name, ConfigurationIdentity identity);

        /// <summary>
        /// Deletes archived files for client before selected data
        /// </summary>
        /// <param name="deletionDate">Date to delete archives before</param>
        /// <param name="identity">Configuration identity being modified</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task DeleteOldArchiveResources(DateTime deletionDate, ConfigurationIdentity identity);
    }
}
