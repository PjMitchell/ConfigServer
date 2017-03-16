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
        public Task<ResourceEntryRequest> GetResource(string name, ConfigurationIdentity identity)
        {
            return Task.FromResult(new ResourceEntryRequest());
        }

        public Task<IEnumerable<ResourceEntryInfo>> GetResourceCatalogue(ConfigurationIdentity identity)
        {
            return Task.FromResult(Enumerable.Empty<ResourceEntryInfo>());
        }

        public Task UpdateResource(UpdateResourceRequest request)
        {
            return Task.FromResult(true);
        }
    }
}
