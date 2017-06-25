using System.Collections.Generic;

namespace ConfigServer.Core
{
    /// <summary>
    /// Snapshot Entry 
    /// </summary>
    public class ConfigurationSnapshotEntry
    {
        /// <summary>
        /// Info for shapshot
        /// </summary>
        public SnapshotEntryInfo Info { get; set; }
        /// <summary>
        /// Configurations in Snapshot
        /// </summary>
        public ICollection<ConfigInstance> Configurations { get; set; }
    }
}
