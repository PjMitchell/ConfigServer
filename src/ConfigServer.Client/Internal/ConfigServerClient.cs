using ConfigServer.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

[assembly: InternalsVisibleTo("ConfigServer.Core.Tests")]
namespace ConfigServer.Client
{
    internal class ConfigServerClient : IConfigServerClient
    {
        private readonly ConfigurationRegistry collection;
        private readonly ConfigServerClientOptions options;
        private readonly IHttpClientWrapper client;
        private readonly IMemoryCache cache;
        private const string cachePrefix = "ConfigServer_";


        public ConfigServerClient(IHttpClientWrapper client, IMemoryCache memorycache, ConfigurationRegistry collection, ConfigServerClientOptions options)
        {
            this.client = client;
            this.collection = collection;
            this.options = options;
            if (memorycache == null && !options.CacheOptions.IsDisabled)
                throw new ArgumentNullException(nameof(memorycache), "Caching is enabled, but IMemoryCache is not registered in service collection. Try adding \"services.AddMemoryCache()\" to startup file");
            this.cache = memorycache;
        }

        public Task<object> BuildConfigAsync(Type type)
        {
            ThrowIfConfigNotRegistered(type);
            if(options.CacheOptions.IsDisabled)
                return GetConfig(type);

            return GetOrAddConfigFromCache(type);

        }

        public async Task<TConfig> BuildConfigAsync<TConfig>() where TConfig : class, new()
        {
            return (TConfig)await BuildConfigAsync(typeof(TConfig));
        }

        public Task<IEnumerable<TConfig>> BuildCollectionConfigAsync<TConfig>() where TConfig : class, new()
        {
            var type = typeof(TConfig);
            ThrowIfConfigNotRegistered(type);
            if (options.CacheOptions.IsDisabled)
                return GetCollectionConfig<TConfig>();

            return GetOrAddCollectionConfigFromCache<TConfig>();
        }

        private Task<IEnumerable<TConfig>> GetOrAddCollectionConfigFromCache<TConfig>() where TConfig : class, new()
        {
            return cache.GetOrCreateAsync(BuildCacheKey(typeof(TConfig)), cacheEntry =>
            {
                if (options.CacheOptions.AbsoluteExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.AbsoluteExpiration.Value);
                if (options.CacheOptions.SlidingExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.SlidingExpiration.Value);
                return GetCollectionConfig<TConfig>();
            });
        }

        private Task<object> GetOrAddConfigFromCache(Type type)
        {
            return cache.GetOrCreateAsync(BuildCacheKey(type), cacheEntry =>
            {
                if (options.CacheOptions.AbsoluteExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.AbsoluteExpiration.Value);
                if (options.CacheOptions.SlidingExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.SlidingExpiration.Value);
                return GetConfig(type);
            });
        }

        private async Task<object> GetConfig(Type type)
        {
            var result = await GetConfig(type.Name);
            return JsonConvert.DeserializeObject(result, type, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        private async Task<IEnumerable<TConfig>> GetCollectionConfig<TConfig>()
        {
            var result = await GetConfig(typeof(TConfig).Name);
            return (IEnumerable<TConfig>)JsonConvert.DeserializeObject(result,typeof(List<TConfig>), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        private async Task<string> GetConfig(string configName)
        {
            var uri = GetUri(configName);
            var response = await client.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
                throw new ConfigServerCommunicationException(uri, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            return content;

        }

        private Uri GetUri(string configName)
        {
            return new Uri($"{options.ConfigServer}/{options.ClientId}/{configName}");
        }

        private void ThrowIfConfigNotRegistered(Type type)
        {
            if (!collection.Any(reg => reg.ConfigType == type))
                throw new InvalidConfigurationException(type);
        }

        private string BuildCacheKey(Type type) => cachePrefix + type.Name;


    }


}
