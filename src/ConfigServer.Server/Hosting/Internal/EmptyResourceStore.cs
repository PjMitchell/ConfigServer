using ConfigServer.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class EmptyResourceStore : IResourceStore
    {
        public Task<ResourceEntry> GetResource(string name, ConfigurationIdentity identity)
        {
            return Task.FromResult(new ResourceEntry());
        }

        public Task<IEnumerable<ResourceEntryInfo>> GetResourceCatalogue(ConfigurationIdentity identity)
        {
            return Task.FromResult(Enumerable.Empty<ResourceEntryInfo>());
        }

        public Task UpdateResource(UpdateResourceRequest request)
        {
            return Task.FromResult(true);
        }

        public Task CopyResources(ConfigurationIdentity sourceIdentity, ConfigurationIdentity destinationIdentity)
        {
            return Task.FromResult(true);
        }

        public Task DeleteResources(string name, ConfigurationIdentity identity)
        {
            return Task.FromResult(true);
        }

        public Task CopyResources(IEnumerable<string> filesToCopy, ConfigurationIdentity sourceIdentity, ConfigurationIdentity destinationIdentity)
        {
            return Task.FromResult(true);
        }
    }
}
