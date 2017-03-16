using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConfigServer.FileProvider
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileResourceStorageConnector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        Task<byte[]> GetResourceAsync(string resourceName, string instanceId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="resource"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        Task SetResourceAsync(string resourceName, byte[] resource, string instanceId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetResourceCatalog(string instanceId);

    }
}