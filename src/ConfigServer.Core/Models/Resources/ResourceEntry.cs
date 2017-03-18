using System.IO;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents a Resource entry request
    /// </summary>
    public class ResourceEntry
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
