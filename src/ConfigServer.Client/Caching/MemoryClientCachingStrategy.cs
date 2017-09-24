using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace ConfigServer.Client
{
    internal class MemoryClientCachingStrategy : IClientCachingStrategy
    {
        private IMemoryCache memorycache;
        private ConfigServerClientOptions options;

        public MemoryClientCachingStrategy(IMemoryCache memorycache, ConfigServerClientOptions options)
        {
            this.memorycache = memorycache;
            this.options = options;
        }

        public Task<TConfig> GetOrCreateAsync<TConfig>(string cacheKey, Func<Task<TConfig>> factory)
        {
            return memorycache.GetOrCreateAsync(cacheKey, cacheEntry =>
            {
                if (options.CacheOptions.AbsoluteExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.AbsoluteExpiration.Value);
                if (options.CacheOptions.SlidingExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.SlidingExpiration.Value);
                return factory();
            });
        }
    }
}
