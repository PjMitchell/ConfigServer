using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Client
{
    internal interface IClientCachingStrategy
    {
        Task<TConfig> GetOrCreateAsync<TConfig>(string cacheKey, Func<Task<TConfig>> factory);
    }
}
