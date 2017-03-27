using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    /// <summary>
    /// Store for ConfigServer Configurations
    /// </summary>
    public interface IConfigRepository : IConfigProvider
    {
        /// <summary>
        /// Saves changes to configuration
        /// </summary>
        /// <param name="config">Updated configuration to be saved</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task UpdateConfigAsync(ConfigInstance config);
    }
}
