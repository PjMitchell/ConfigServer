using System;

namespace ConfigServer.Core
{
    /// <summary>
    /// Info on Snapshot
    /// </summary>
    public class SnapshotEntryInfo
    {
        /// <summary>
        /// Id of snapshot
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of snapshot
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Group Id for snapshot
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Time stamp of snapshot
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}
