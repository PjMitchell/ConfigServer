using System;
using System.Threading.Tasks;

namespace ConfigServer.Client
{
    internal class NoCachingStrategy : IClientCachingStrategy
    {
        public Task<TConfig> GetOrCreateAsync<TConfig>(string cacheKey, Func<Task<TConfig>> factory) => factory();
    }
}
