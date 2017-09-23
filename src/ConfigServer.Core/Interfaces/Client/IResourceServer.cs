using System;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    /// <summary>
    /// Client for ConfigServer Resource service
    /// </summary>
    public interface IResourceServer
    {
        /// <summary>
        /// Gets resource file
        /// </summary>
        /// <param name="name">Name of resource</param>
        /// <returns>Resource response</returns>
        Task<ResourceEntry> GetResourceAsync(string name);

        /// <summary>
        /// Get resource uri
        /// </summary>
        /// <param name="name">Name of resource</param>
        /// <returns>Resource uri</returns>
        Uri GetResourceUri(string name);
    }
}
