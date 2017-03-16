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
        /// <returns>Resource response</returns>
        Task<UpdateResourceResponse> GetResource(string name, ConfigurationIdentity identity);

        /// <summary>
        /// Updates resource
        /// </summary>
        /// <param name="request">Update request </param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task UpdateResource(UpdateResourceRequest request);

        /// <summary>
        /// Copies all files between two identities
        /// </summary>
        /// <param name="sourceIdentity">source identity to be copied</param>
        /// <param name="destinationIdentity">target identity</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task CopyResources(ConfigurationIdentity sourceIdentity, ConfigurationIdentity destinationIdentity);

        /// <summary>
        /// Copies files between two identities
        /// </summary>
        /// <param name="filesToCopy">Files to be copied</param>
        /// <param name="sourceIdentity">source identity to be copied</param>
        /// <param name="destinationIdentity">target identity</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task CopyResources(IEnumerable<string> filesToCopy, ConfigurationIdentity sourceIdentity, ConfigurationIdentity destinationIdentity);

        /// <summary>
        /// Deletes file for client
        /// </summary>
        /// <param name="name">Name of file to be seleted</param>
        /// <param name="identity">Configuration identity being modified</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task DeleteResources(string name, ConfigurationIdentity identity);
    }
}
