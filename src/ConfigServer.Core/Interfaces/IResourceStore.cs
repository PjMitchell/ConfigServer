using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    /// <summary>
    /// Store for resources
    /// </summary>
    public interface IResourceStore
    {
        /// <summary>
        /// Returns all ResourceEntryInfos for a configuration identity
        /// </summary>
        /// <param name="identity">Configuration identity being queryied</param>
        /// <returns>All ResourceEntryInfos for a configuration identity</returns>
        Task<IEnumerable<ResourceEntryInfo>> GetResourceCatalogue(ConfigurationIdentity identity);

        /// <summary>
        /// Gets resource for a configuration identity
        /// </summary>
        /// <param name="name">Name of resource</param>
        /// <param name="identity">Configuration identity being queryied</param>
        /// <returns>Resource request</returns>
        Task<ResourceEntryRequest> GetResource(string name, ConfigurationIdentity identity);

        /// <summary>
        /// Updates resource
        /// </summary>
        /// <param name="request">Update request </param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task UpdateResource(UpdateResourceRequest request);
    }
}
