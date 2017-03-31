using ConfigServer.Core;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.TextProvider.Core
{
    /// <summary>
    /// Text storage implementation of IConfigRepository
    /// </summary>
    public class TextStorageConfigurationRepository : IConfigRepository
    {

        readonly JsonSerializerSettings jsonSerializerSettings;
        readonly IMemoryCache memoryCache;
        readonly IStorageConnector storageConnector;
        private const string cachePrefix = "ConfigServer_ConfigRepository_";

        /// <summary>
        /// Initializes File store
        /// </summary>
        public TextStorageConfigurationRepository(IMemoryCache memoryCache,IStorageConnector storageConnector, ITextStorageSetting options)
        {
            jsonSerializerSettings = options.JsonSerializerSettings;
            this.memoryCache = memoryCache;
            this.storageConnector = storageConnector;
        }



        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>ConfigInstance of the type requested</returns>
        public async Task<ConfigInstance> GetAsync(Type type, ConfigurationIdentity id)
        {
            var configId = type.Name;
            var configPath = GetCacheKey(configId, id.ClientId);
            var result = ConfigFactory.CreateGenericInstance(type, id.ClientId);
            var json = await memoryCache.GetOrCreateAsync(cachePrefix + configPath, e => {
                e.SetSlidingExpiration(TimeSpan.FromMinutes(5));
                return storageConnector.GetConfigFileAsync(type.Name, id.ClientId);
            });

            if (!string.IsNullOrWhiteSpace(json))
                result.SetConfiguration(JsonConvert.DeserializeObject(json, type, jsonSerializerSettings));
            return result;
        }

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <typeparam name="TConfiguration">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>ConfigInstance of the type requested</returns>
        public async Task<ConfigInstance<TConfiguration>> GetAsync<TConfiguration>(ConfigurationIdentity id) where TConfiguration : class, new()
        {
            var result = await GetAsync(typeof(TConfiguration), id);
            return (ConfigInstance<TConfiguration>)result;
        }

        /// <summary>
        /// Gets Collection Configuration
        /// </summary>
        /// <typeparam name="TConfiguration">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Enumerable of the type requested</returns>
        public async Task<IEnumerable<TConfiguration>> GetCollectionAsync<TConfiguration>(ConfigurationIdentity id) where TConfiguration : class, new()
        {
            var config = await GetCollectionAsync(typeof(TConfiguration), id);
            return (IEnumerable<TConfiguration>)config;
        }

        /// <summary>
        /// Gets Collection Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Enumerable of the type requested</returns>
        public async Task<IEnumerable> GetCollectionAsync(Type type, ConfigurationIdentity id)
        {
            var configId = type.Name;
            var configPath = GetCollectionConfigCacheKey(configId, id.ClientId);

            var json = await memoryCache.GetOrCreateAsync(cachePrefix + configPath, e => {
                e.SetSlidingExpiration(TimeSpan.FromMinutes(5));
                return storageConnector.GetConfigFileAsync(type.Name, id.ClientId);
            });
            var configType = BuildGenericType(typeof(List<>), type);
            if (!string.IsNullOrWhiteSpace(json))
                 return (IEnumerable)JsonConvert.DeserializeObject(json, configType, jsonSerializerSettings);
            return (IEnumerable)Activator.CreateInstance(configType);
        }



        /// <summary>
        /// Saves changes to configuration
        /// </summary>
        /// <param name="config">Updated configuration to be saved</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task UpdateConfigAsync(ConfigInstance config)
        {
            var configId = config.ConfigType.Name;
            var configPath = GetCacheKey(configId, config.ClientId);
            var configText = JsonConvert.SerializeObject(config.GetConfiguration(), jsonSerializerSettings);
            await storageConnector.SetConfigFileAsync(configId, config.ClientId,configText);
            memoryCache.Set<string>(cachePrefix + configPath, configText, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5)));            
        }

        private string GetCacheKey(string configId, string clientId) => $"{clientId}_{configId}";

        private string GetCollectionConfigCacheKey(string configId, string clientId) => $"Collection_{clientId}_{configId}";

        private static Type BuildGenericType(Type genericType, params Type[] typeArgs)
        {
            return genericType.MakeGenericType(typeArgs);
        }
    }
}
