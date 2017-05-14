using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class EmptyResourceArchive : IResourceArchive
    {
        public Task DeleteArchiveResource(string name, ConfigurationIdentity identity)
        {
            return Task.FromResult(true);
        }

        public Task DeleteOldArchiveResources(DateTime deletionDate, ConfigurationIdentity identity)
        {
            return Task.FromResult(true);
        }

        public Task<ResourceEntry> GetArchiveResource(string name, ConfigurationIdentity identity)
        {
            return Task.FromResult(new ResourceEntry());
        }

        public Task<IEnumerable<ResourceEntryInfo>> GetArchiveResourceCatalogue(ConfigurationIdentity identity)
        {
            return Task.FromResult(Enumerable.Empty<ResourceEntryInfo>());
        }
    }
}
