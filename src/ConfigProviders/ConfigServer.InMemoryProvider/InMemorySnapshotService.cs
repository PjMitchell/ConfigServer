using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.InMemoryProvider
{
    internal class InMemorySnapshotService : IConfigurationSnapshotService
    {
        private Dictionary<SnapshotEntryInfo, ICollection<ConfigInstance>> source;

        public InMemorySnapshotService()
        {
            source = new Dictionary<SnapshotEntryInfo, ICollection<ConfigInstance>>(new InfoComparer());
        }

        public Task<ConfigurationSnapshotEntry> GetSnapshot(string snapshotId, ConfigurationIdentity targetConfigurationIdentity)
        {
            var entry = source.SingleOrDefault(s => string.Equals(snapshotId, s.Key.Id));
            var result = new ConfigurationSnapshotEntry { Info = entry.Key, Configurations = entry.Value };
            return Task.FromResult(result);
        }

        public Task<IEnumerable<SnapshotEntryInfo>> GetSnapshots()
        {
            return Task.FromResult<IEnumerable<SnapshotEntryInfo>>(source.Keys);
        }

        public Task SaveSnapshot(ConfigurationSnapshotEntry snapshot)
        {
            source[snapshot.Info] = snapshot.Configurations;
            return Task.FromResult(0);
        }

        private class InfoComparer : IEqualityComparer<SnapshotEntryInfo>
        {
            public bool Equals(SnapshotEntryInfo x, SnapshotEntryInfo y)
            {
                return string.Equals(x.Id, y.Id, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(SnapshotEntryInfo obj)
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Id);
            }
        }
    }
}
