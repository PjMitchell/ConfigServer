using System.Threading.Tasks;

namespace ConfigServer.TextProvider.Core
{
    /// <summary>
    /// Interface for storage connector
    /// </summary>
    public interface IStorageConnector
    {
        /// <summary>
        /// Gets Configuration File from storage
        /// </summary>
        /// <param name="configId">Id of configuration</param>
        /// <param name="instanceId">Id of configuration instance</param>
        /// <returns>Text from config file, empty string if no config found</returns>
        Task<string> GetConfigFileAsync(string configId, string instanceId);

        /// <summary>
        /// Set Configuration File in storage
        /// </summary>
        /// <param name="configId">Id of configuration</param>
        /// <param name="instanceId">Id of configuration</param>
        /// <param name="value">new value</param>
        /// <returns>Task from operation</returns>
        Task SetConfigFileAsync(string configId, string instanceId, string value);

        /// <summary>
        /// Gets Client Registry File from storage
        /// </summary>
        /// <returns>Registry File from storage</returns>
        Task<string> GetClientRegistryFileAsync();

        /// <summary>
        /// Set Client Registry File in storage
        /// </summary>
        /// <param name="value">new value</param>
        /// <returns>Task from operation</returns>
        Task SetClientRegistryFileAsync(string value);

        /// <summary>
        /// Gets Client Group Registry File from storage
        /// </summary>
        /// <returns>Registry File from storage</returns>
        Task<string> GetClientGroupRegistryFileAsync();

        /// <summary>
        /// Set Client Group Registry File in storage
        /// </summary>
        /// <param name="value">new value</param>
        /// <returns>Task from operation</returns>
        Task SetClientGroupRegistryFileAsync(string value);
    }
}
