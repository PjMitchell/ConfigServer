using System;
using System.IO;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents a Config Archive entry request
    /// </summary>
    public class ConfigArchiveEntry
    {
        /// <summary>
        /// Was requested entry found
        /// </summary>
        public bool HasEntry { get; set; }
        /// <summary>
        /// Name of config archive
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Config
        /// </summary>
        public string Content { get; set; }
    }
}
