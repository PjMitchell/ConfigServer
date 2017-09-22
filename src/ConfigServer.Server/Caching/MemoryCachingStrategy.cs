using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    /// <summary>
    /// Strategy for caching items for configuration server using IMemoryCache
    /// </summary>
    public class MemoryCachingStrategy : ICachingStrategy
    {
        private readonly IMemoryCache cache;

        /// <summary>
        /// Constructor for MemoryCachingStrategy
        /// </summary>
        /// <param name="cache">cache used by strategy</param>
        public MemoryCachingStrategy(IMemoryCache cache)
        {
            this.cache = cache;
        }

        /// <summary>
        /// Gets from cache or creates item and adds it to the cache
        /// </summary>
        /// <typeparam name="TItem">Type of Item being cached</typeparam>
        /// <param name="key">Key for cached item</param>
        /// <param name="factory">Item factory</param>
        /// <returns>Item from cache or factory</returns>
        public Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> factory)
        {
            return cache.GetOrCreateAsync(key, e => {
                e.SetSlidingExpiration(TimeSpan.FromMinutes(5));
                return factory();
            });
        }

        /// <summary>
        /// Removes item from cache
        /// </summary>
        /// <param name="key">Key for cached item</param>
        /// <returns>Task representing operation</returns>
        public Task Remove(string key)
        {
            cache.Remove(key);
            return Task.FromResult(true);
        }
    }
}
