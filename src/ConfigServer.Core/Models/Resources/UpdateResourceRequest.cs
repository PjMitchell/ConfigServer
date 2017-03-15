using System.IO;

namespace ConfigServer.Core
{
    /// <summary>
    /// Request to update Resource
    /// </summary>
    public class UpdateResourceRequest
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
    }
}
