using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    /// <summary>
    /// Strategy for caching items for configuration server using IDistributedCache
    /// </summary>
    public class DistributedCachingStrategy : ICachingStrategy
    {
        private readonly IDistributedCache cache;
        private readonly DistributedCacheEntryOptions options;

        /// <summary>
        /// Constructor for DistributedCachingStrategy
        /// </summary>
        /// <param name="cache">cache used by strategy</param>
        public DistributedCachingStrategy(IDistributedCache cache)
        {
            this.cache = cache;
            this.options = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) };
        }

        /// <summary>
        /// Gets from cache or creates item and adds it to the cache
        /// </summary>
        /// <typeparam name="TItem">Type of Item being cached</typeparam>
        /// <param name="key">Key for cached item</param>
        /// <param name="factory">Item factory</param>
        /// <returns>Item from cache or factory</returns>
        public async Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> factory)
        {
            var value = await cache.GetAsync(key);
            if (value != null)
                return JsonConvert.DeserializeObject<TItem>(Encoding.UTF8.GetString(value));
            var result = await factory();
            await cache.SetAsync(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result)), options);
            return result;
        }

        /// <summary>
        /// Removes item from cache
        /// </summary>
        /// <param name="key">Key for cached item</param>
        /// <returns>Task representing operation</returns>
        public Task Remove(string key)
        {
            return cache.RemoveAsync(key);
        }
    }
}
