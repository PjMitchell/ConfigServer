using ConfigServer.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
namespace ConfigServer.Client
{
    /// <summary>
    /// Config Server Client
    /// </summary>
    public class ConfigServerClient : IConfigServer
    {
        private readonly IConfigurationRegistry collection;
        private readonly ConfigServerClientOptions options;
        private readonly IHttpClientWrapper client;
        private readonly IMemoryCache cache;
        private const string cachePrefix = "ConfigServer_";

        /// <summary>
        /// Constructs new Config Server Client
        /// </summary>
        /// <param name="client">HttpClient Wrapper</param>
        /// <param name="memorycache">Memory cache</param>
        /// <param name="collection">Configuration Registry</param>
        /// <param name="options">ConfigServerClientOptions</param>
        public ConfigServerClient(IHttpClientWrapper client, IMemoryCache memorycache, IConfigurationRegistry collection, ConfigServerClientOptions options)
        {
            this.client = client;
            this.collection = collection;
            this.options = options;
            if (memorycache == null && !options.CacheOptions.IsDisabled)
                throw new ArgumentNullException(nameof(memorycache), "Caching is enabled, but IMemoryCache is not registered in service collection. Try adding \"services.AddMemoryCache()\" to startup file");
            cache = memorycache;
        }

        /// <summary>
        /// Builds Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be build</param>
        /// <returns>Configuration of specified type</returns>
        public async Task<object> GetConfigAsync(Type type)
        {
            var registration = GetRegistration(type);
            return options.CacheOptions.IsDisabled
                ? await GetConfigInternal(registration).ConfigureAwait(false)
                : await GetOrAddConfigFromCache(registration).ConfigureAwait(false);            

        }

        /// <summary>
        /// Gets Configuration that is a collection
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be build</typeparam>
        /// <returns>IEnumerable of configuration of specified type</returns>
        public async Task<TConfig> GetConfigAsync<TConfig>() where TConfig : class, new()
        {
            return (TConfig)await GetConfigAsync(typeof(TConfig)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets Configuration that is a collection
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be build</typeparam>
        /// <returns>IEnumerable of configuration of specified type</returns>
        public async Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>() where TConfig : class, new()
        {
            var type = typeof(TConfig);
            var registration = GetRegistration(type);

            return options.CacheOptions.IsDisabled
                ? await GetCollectionConfigInternal<TConfig>(registration).ConfigureAwait(false)
                : await GetOrAddCollectionConfigFromCache<TConfig>(registration).ConfigureAwait(false);
        }

        private Task<IEnumerable<TConfig>> GetOrAddCollectionConfigFromCache<TConfig>(ConfigurationRegistration registration) where TConfig : class, new()
        {
            return cache.GetOrCreateAsync(BuildCacheKey(typeof(TConfig)), cacheEntry =>
            {
                if (options.CacheOptions.AbsoluteExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.AbsoluteExpiration.Value);
                if (options.CacheOptions.SlidingExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.SlidingExpiration.Value);
                return GetCollectionConfigInternal<TConfig>(registration);
            });
        }

        private Task<object> GetOrAddConfigFromCache(ConfigurationRegistration registration)
        {
            return cache.GetOrCreateAsync(BuildCacheKey(registration.ConfigType), cacheEntry =>
            {
                if (options.CacheOptions.AbsoluteExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.AbsoluteExpiration.Value);
                if (options.CacheOptions.SlidingExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.SlidingExpiration.Value);
                return GetConfigInternal(registration);
            });
        }

        private async Task<object> GetConfigInternal(ConfigurationRegistration registration)
        {
            var result = await GetConfig(registration.ConfigurationName);
            return JsonConvert.DeserializeObject(result, registration.ConfigType, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        private async Task<IEnumerable<TConfig>> GetCollectionConfigInternal<TConfig>(ConfigurationRegistration registration)
        {
            var result = await GetConfig(registration.ConfigurationName);
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

        private ConfigurationRegistration GetRegistration(Type type)
        {
            if (collection.TryGet(type, out var result))
                return result;
            throw new InvalidConfigurationException(type);
        }

        private string BuildCacheKey(Type type) => cachePrefix + type.Name;

    }
}
