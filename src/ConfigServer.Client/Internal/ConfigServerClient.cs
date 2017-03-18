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
        private readonly ConfigurationRegistry collection;
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
        public ConfigServerClient(IHttpClientWrapper client, IMemoryCache memorycache, ConfigurationRegistry collection, ConfigServerClientOptions options)
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
            ThrowIfConfigNotRegistered(type);
            return options.CacheOptions.IsDisabled
                ? await GetConfigInternal(type).ConfigureAwait(false)
                : await GetOrAddConfigFromCache(type).ConfigureAwait(false);            

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
            ThrowIfConfigNotRegistered(type);

            return options.CacheOptions.IsDisabled
                ? await GetCollectionConfigInternal<TConfig>().ConfigureAwait(false)
                : await GetOrAddCollectionConfigFromCache<TConfig>().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be build</typeparam>
        /// <returns>Configuration of specified type</returns>
        public TConfig GetConfig<TConfig>() where TConfig : class, new()
        {
            return GetConfigAsync<TConfig>().Result;
        }

        /// <summary>
        /// Gets Configuration that is a collection
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be build</typeparam>
        /// <returns>IEnumerable of configuration of specified type</returns>
        public IEnumerable<TConfig> GetCollectionConfig<TConfig>() where TConfig : class, new()
        {
            return GetCollectionConfigAsync<TConfig>().Result;
        }

        /// <summary>
        /// Builds Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be build</param>
        /// <returns>Configuration of specified type</returns>
        public object GetConfig(Type type)
        {
            return GetConfigAsync(type).Result;
        }

        /// <summary>
        /// Gets resource file
        /// </summary>
        /// <param name="name">Name of resource</param>
        /// <returns>Resource response</returns>
        public async Task<ResourceEntry> GetResourceAsync(string name)
        {
            var uri = GetResourceUri(name);
            var response = await client.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new ResourceEntry { Name = name };

            if (!response.IsSuccessStatusCode)
                throw new ConfigServerCommunicationException(uri, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            return new ResourceEntry { Name = name, HasEntry = true, Content = await response.Content.ReadAsStreamAsync() }; ;
        }

        /// <summary>
        /// Gets resource file
        /// </summary>
        /// <param name="name">Name of resource</param>
        /// <returns>Resource response</returns>
        public ResourceEntry GetResource(string name)
        {
            return GetResourceAsync(name).Result;
        }

        /// <summary>
        /// Get resource uri
        /// </summary>
        /// <param name="name">Name of resource</param>
        /// <returns>Resource uri</returns>
        public Uri GetResourceUri(string name)
        {
            return new Uri($"{options.ConfigServer}/Resource/{options.ClientId}/{name}");
        }

        private Task<IEnumerable<TConfig>> GetOrAddCollectionConfigFromCache<TConfig>() where TConfig : class, new()
        {
            return cache.GetOrCreateAsync(BuildCacheKey(typeof(TConfig)), cacheEntry =>
            {
                if (options.CacheOptions.AbsoluteExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.AbsoluteExpiration.Value);
                if (options.CacheOptions.SlidingExpiration.HasValue)
                    cacheEntry.SetAbsoluteExpiration(options.CacheOptions.SlidingExpiration.Value);
                return GetCollectionConfigInternal<TConfig>();
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
                return GetConfigInternal(type);
            });
        }

        private async Task<object> GetConfigInternal(Type type)
        {
            var result = await GetConfig(type.Name);
            return JsonConvert.DeserializeObject(result, type, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        private async Task<IEnumerable<TConfig>> GetCollectionConfigInternal<TConfig>()
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
