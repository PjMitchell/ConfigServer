using ConfigServer.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
namespace ConfigServer.Client
{

    internal class ConfigServerClient : IConfigServer
    {
        private readonly IConfigurationRegistry collection;
        private readonly ConfigServerClientOptions options;
        private readonly IHttpClientWrapper client;
        private readonly IMemoryCache cache;
        private const string cachePrefix = "ConfigServer_";

        public ConfigServerClient(IHttpClientWrapper client, IMemoryCache memorycache, IConfigurationRegistry collection, ConfigServerClientOptions options)
        {
            this.client = client;
            this.collection = collection;
            this.options = options;
            if (memorycache == null && !options.CacheOptions.IsDisabled)
                throw new ArgumentNullException(nameof(memorycache), "Caching is enabled, but IMemoryCache is not registered in service collection. Try adding \"services.AddMemoryCache()\" to startup file");
            cache = memorycache;
        }

        public Task<object> GetConfigAsync(Type type)
        {
            return GetConfigAsync(type, options.ClientId);
        }

        public async Task<object> GetConfigAsync(Type type, string clientId)
        {
            var registration = GetRegistration(type);
            return options.CacheOptions.IsDisabled
                ? await GetConfigInternal(registration, clientId).ConfigureAwait(false)
                : await GetOrAddConfigFromCache(registration, clientId).ConfigureAwait(false);
        }

        public Task<TConfig> GetConfigAsync<TConfig>() where TConfig : class, new()
        {
            return GetConfigAsync<TConfig>(options.ClientId);
        }

        public async Task<TConfig> GetConfigAsync<TConfig>(string clientId) where TConfig : class, new()
        {
            return (TConfig)await GetConfigAsync(typeof(TConfig), clientId).ConfigureAwait(false);
        }

        public Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>() where TConfig : class, new()
        {
            return GetCollectionConfigAsync<TConfig>(options.ClientId);
        }

        public async Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>(string clientId) where TConfig : class, new()
        {
            var type = typeof(TConfig);
            var registration = GetRegistration(type);

            return options.CacheOptions.IsDisabled
                ? await GetCollectionConfigInternal<TConfig>(registration, clientId).ConfigureAwait(false)
                : await GetOrAddCollectionConfigFromCache<TConfig>(registration, clientId).ConfigureAwait(false);
        }

        private Task<IEnumerable<TConfig>> GetOrAddCollectionConfigFromCache<TConfig>(ConfigurationRegistration registration, string clientId) where TConfig : class, new()
        {
            return cache.GetOrCreateAsync(BuildCacheKey(typeof(TConfig)), cacheEntry =>
            {
                if (options.CacheOptions.AbsoluteExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.AbsoluteExpiration.Value);
                if (options.CacheOptions.SlidingExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.SlidingExpiration.Value);
                return GetCollectionConfigInternal<TConfig>(registration, clientId);
            });
        }

        private Task<object> GetOrAddConfigFromCache(ConfigurationRegistration registration, string clientId)
        {
            return cache.GetOrCreateAsync(BuildCacheKey(registration.ConfigType), cacheEntry =>
            {
                if (options.CacheOptions.AbsoluteExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.AbsoluteExpiration.Value);
                if (options.CacheOptions.SlidingExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.SlidingExpiration.Value);
                return GetConfigInternal(registration, clientId);
            });
        }

        private async Task<object> GetConfigInternal(ConfigurationRegistration registration, string clientId)
        {
            var result = await GetConfig(registration.ConfigurationName, clientId);
            return JsonConvert.DeserializeObject(result, registration.ConfigType, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        private async Task<IEnumerable<TConfig>> GetCollectionConfigInternal<TConfig>(ConfigurationRegistration registration, string clientId)
        {
            var result = await GetConfig(registration.ConfigurationName, clientId);
            return (IEnumerable<TConfig>)JsonConvert.DeserializeObject(result,typeof(List<TConfig>), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        private async Task<string> GetConfig(string configName, string clientId)
        {
            var uri = GetUri(configName, clientId);
            var response = await client.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
                throw new ConfigServerCommunicationException(uri, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            return content;

        }

        private Uri GetUri(string configName, string clientId)
        {
            return new Uri($"{options.ConfigServer}/{clientId}/{configName}");
        }

        private ConfigurationRegistration GetRegistration(Type type)
        {
            if (collection.TryGet(type, out var result))
                return result;
            throw new InvalidConfigurationException(type);
        }

        private string BuildCacheKey(Type type) => cachePrefix + type.Name;

    }
}
