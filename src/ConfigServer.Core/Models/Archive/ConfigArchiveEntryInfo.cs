using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Core
{
    /// <summary>
    /// Information about a Config Archive entry
    /// </summary>
    public class ConfigArchiveEntryInfo
    {
        /// <summary>
        /// Name of Config Entry
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of Config
        /// </summary>
        public string Configuration { get; set; }
        
        /// <summary>
        /// Time stamp for Config
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Time stamp for Archive
        /// </summary>
        public DateTime ArchiveTimeStamp { get; set; }

        public string ServerVersion { get; set; }

    }
}
