using ConfigServer.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace ConfigServer.FileProvider
{
    /// <summary>
    /// 
    /// </summary>
    internal class FileResourceStore : IResourceStore
    {
        private IFileResourceStorageConnector fileResourceConnector;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileResourceConnector"></param>
        public FileResourceStore(IFileResourceStorageConnector fileResourceConnector)
        {
            this.fileResourceConnector = fileResourceConnector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<ResourceEntry> GetResource(string name, ConfigurationIdentity identity)
        {
            var buffer = await fileResourceConnector.GetResourceAsync(name, identity.ClientId);

            return new ResourceEntry()
            {
                Name = name,
                Content = new MemoryStream(buffer),
                HasEntry = buffer.Length == 0 ? false : true
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ResourceEntryInfo>> GetResourceCatalogue(ConfigurationIdentity identity)
        {
            var entries = await fileResourceConnector.GetResourceCatalog(identity.ClientId);

            return entries.Select(e => new ResourceEntryInfo()
            {
                Name = e
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task UpdateResource(UpdateResourceRequest request)
        {
            MemoryStream stream = new MemoryStream();
            await request.Content.CopyToAsync(stream);
            await fileResourceConnector.SetResourceAsync(request.Name, stream.ToArray(), request.Identity.ClientId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceIdentity"></param>
        /// <param name="destinationIdentity"></param>
        /// <returns></returns>
        public async Task CopyResources(ConfigurationIdentity sourceIdentity, ConfigurationIdentity destinationIdentity)
        {
            await fileResourceConnector.CopyResourcesAsync(sourceIdentity.ClientId, destinationIdentity.ClientId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task DeleteResources(string name, ConfigurationIdentity identity)
        {
            await fileResourceConnector.DeleteResources(name, identity.ClientId);
        }

        public Task CopyResources(IEnumerable<string> filesToCopy, ConfigurationIdentity sourceIdentity, ConfigurationIdentity destinationIdentity)
        {
            return fileResourceConnector.CopyResourcesAsync(filesToCopy, sourceIdentity.ClientId, destinationIdentity.ClientId);
        }
    }
}
