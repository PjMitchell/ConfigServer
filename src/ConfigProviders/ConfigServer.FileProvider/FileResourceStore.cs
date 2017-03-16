using ConfigServer.Core;
using System;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace ConfigServer.FileProvider
{
    /// <summary>
    /// 
    /// </summary>
    public class FileResourceStore : IResourceStore
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

        public async Task<UpdateResourceResponse> GetResource(string name, ConfigurationIdentity identity)
        {
            var buffer = await fileResourceConnector.GetResourceAsync(name, identity.ClientId);

            return new UpdateResourceResponse()
            {
                Name = name,
                Content = new MemoryStream(buffer),
                HasEntry = buffer.Length == 0 ? false : true
            };
        }

        public async Task<IEnumerable<ResourceEntryInfo>> GetResourceCatalogue(ConfigurationIdentity identity)
        {
            var entries = await fileResourceConnector.GetResourceCatalog(identity.ClientId);

            return entries.Select(e => new ResourceEntryInfo()
            {
                Name = e
            });
        }

        public async Task UpdateResource(UpdateResourceRequest request)
        {
            MemoryStream stream = new MemoryStream();
            await request.Content.CopyToAsync(stream);
            await fileResourceConnector.SetResourceAsync(request.Name, stream.ToArray(), request.Identity.ClientId);
        }
    }
}
