using System.IO;

namespace ConfigServer.Core
{
    /// <summary>
    /// Response from update Resource
    /// </summary>
    public class UpdateResourceResponse
    {
        /// <summary>
        /// Name of Resource
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Configuration identity for resource
        /// </summary>
        public ConfigurationIdentity Identity { get; set; }
        /// <summary>
        /// Resource content
        /// </summary>
        public Stream Content { get; set; }

        /// <summary>
        /// True if entry exists
        /// </summary>
        public bool HasEntry { get; set; }
    }
}
