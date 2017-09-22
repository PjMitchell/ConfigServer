using System;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    /// <summary>
    /// Strategy for caching items for configuration server
    /// </summary>
    public interface ICachingStrategy
    {
        /// <summary>
        /// Gets from cache or creates item and adds it to the cache
        /// </summary>
        /// <typeparam name="TItem">Type of Item being cached</typeparam>
        /// <param name="key">Key for cached item</param>
        /// <param name="factory">Item factory</param>
        /// <returns>Item from cache or factory</returns>
        Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> factory);

        /// <summary>
        /// Removes item from cache
        /// </summary>
        /// <param name="key">Key for cached item</param>
        /// <returns>Task representing operation</returns>
        Task Remove(string key);
    }
}
