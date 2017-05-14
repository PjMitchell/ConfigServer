using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Core
{
    /// <summary>
    /// Information about a Resource entry
    /// </summary>
    public class ResourceEntryInfo
    {
        /// <summary>
        /// Name of Resource Entry
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Time stamp for resource
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// User who uploaded resource
        /// </summary>
        public string UserUpdated { get; set; }
    }
}
