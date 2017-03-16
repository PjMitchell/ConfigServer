﻿using ConfigServer.Core;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
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
    }
}