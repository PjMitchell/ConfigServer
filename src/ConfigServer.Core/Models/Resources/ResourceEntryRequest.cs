using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents a Resource entry request
    /// </summary>
    public class ResourceEntryRequest
    {
        /// <summary>
        /// Was requested entry found
        /// </summary>
        public bool HasEntry { get; set; }
        /// <summary>
        /// Name of resource
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Resource Content
        /// </summary>
        public Stream Content { get; set; }
    }
}
