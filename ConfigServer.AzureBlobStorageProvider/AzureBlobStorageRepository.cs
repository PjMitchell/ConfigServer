using ConfigServer.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace ConfigServer.AzureBlobStorageProvider
{
    /// <summary>
    /// Azure Blob Storage Repository implementation of IConfigRepository
    /// </summary>
    public class AzureBlobStorageRepository : IConfigRepository
    {
        private readonly IStorageConnector storageContainer;
        private readonly AzureBlobStorageRepositoryBuilderOptions options;
        const string indexFile = "clientIndex.json";
        private string cachePrefix = "ConfigServer_AzureBlobStorageRepository_";
        readonly IMemoryCache memoryCache;

        /// <summary>
        /// Initializes Azure Blob Storage Repository
        /// </summary>
        /// <param name="storageContainer">storage connector for repository</param>
        /// <param name="memoryCache">Memory Cache of repository</param>
        /// <param name="options">Options repository</param>
        public AzureBlobStorageRepository(IStorageConnector storageContainer, IMemoryCache memoryCache, AzureBlobStorageRepositoryBuilderOptions options)
        {
            this.memoryCache = memoryCache;
            this.storageContainer = storageContainer;
            this.options = options;
        }

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>ConfigInstance of the type requested</returns>
        public Task<ConfigInstance> GetAsync(Type type, ConfigurationIdentity id)
        {
            
            var configPath = GetConfigPath(type, id.ClientId);
            return memoryCache.GetOrCreateAsync(cachePrefix + configPath, e => {
                e.SetSlidingExpiration(TimeSpan.FromMinutes(5));
                return GetInternal(configPath, type, id);
            });

        }

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>ConfigInstance of the type requested</returns>
        public async Task<ConfigInstance<TConfig>> GetAsync<TConfig>(ConfigurationIdentity id) where TConfig : class, new()
        {
            var result = await GetAsync(typeof(TConfig), id);
            return (ConfigInstance<TConfig>)result;
        }

        /// <summary>
        /// Get all Client in store
        /// </summary>
        /// <returns>Available Client</returns>
        public async Task<IEnumerable<ConfigurationClient>> GetClientsAsync()
        {
            var file = await storageContainer.GetFileAsync(indexFile);
            if (string.IsNullOrWhiteSpace(file))
                return Enumerable.Empty<ConfigurationClient>();
            return JsonConvert.DeserializeObject<List<ConfigurationClient>>(file, options.JsonSerializerSettings);
        }

        /// <summary>
        /// Creates or updates client details in store
        /// </summary>
        /// <param name="client">Updated Client detsils</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        public async Task UpdateClientAsync(ConfigurationClient client)
        {
            var clients = await GetClientsAsync();
            var clientDic = clients.ToDictionary(k => k.ClientId);
            clientDic[client.ClientId] = client;
            await storageContainer.SetFileAsync(indexFile, JsonConvert.SerializeObject(clientDic.Values.ToList()));
        }

        /// <summary>
        /// Saves changes to configuration
        /// </summary>
        /// <param name="config">Updated configuration to be saved</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task UpdateConfigAsync(ConfigInstance config)
        {
            var configPath = GetConfigPath(config.ConfigType, config.ClientId);
            await storageContainer.SetFileAsync(configPath, JsonConvert.SerializeObject(config.GetConfiguration(), options.JsonSerializerSettings));
            memoryCache.Set<ConfigInstance>(cachePrefix + configPath, config, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5)));
        }

        private async Task<ConfigInstance> GetInternal(string path, Type type, ConfigurationIdentity id)
        {
            var result = ConfigFactory.CreateGenericInstance(type, id.ClientId);
            var file = await storageContainer.GetFileAsync(path);
            if (!string.IsNullOrWhiteSpace(file))
                result.SetConfiguration(JsonConvert.DeserializeObject(file, type, options.JsonSerializerSettings));
            return result;
        }

        private string GetConfigPath(Type configType, string configSetId)
        {
            return $"{configSetId}/{configType.Name}.json";
        }
    }
}
