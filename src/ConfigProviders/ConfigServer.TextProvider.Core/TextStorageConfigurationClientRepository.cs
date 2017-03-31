using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace ConfigServer.TextProvider.Core
{
    /// <summary>
    /// Text storage implementation of IConfigClientRepository
    /// </summary>
    public class TextStorageConfigurationClientRepository : IConfigClientRepository
    {
        readonly JsonSerializerSettings jsonSerializerSettings;
        readonly IMemoryCache memoryCache;
        readonly IStorageConnector storageConnector;
        private const string cachePrefix = "ConfigServer_ConfigClientRepository_";

        /// <summary>
        /// Initializes File store
        /// </summary>
        public TextStorageConfigurationClientRepository(IMemoryCache memoryCache, IStorageConnector storageConnector, ITextStorageSetting options)
        {
            jsonSerializerSettings = options.JsonSerializerSettings;
            this.memoryCache = memoryCache;
            this.storageConnector = storageConnector;
        }

        /// <summary>
        /// Get all Client Groups in store
        /// </summary>
        /// <returns>Available Client Groups</returns>
        public async Task<IEnumerable<ConfigurationClientGroup>> GetClientGroupsAsync()
        {
            var json = await storageConnector.GetClientGroupRegistryFileAsync();
            return JsonConvert.DeserializeObject<List<ConfigurationClientGroup>>(json, jsonSerializerSettings) ?? Enumerable.Empty<ConfigurationClientGroup>();
        }

        /// <summary>
        /// Get all Client in store
        /// </summary>
        /// <returns>Available Client</returns>
        public async Task<IEnumerable<ConfigurationClient>> GetClientsAsync()
        {
            var json = await storageConnector.GetClientRegistryFileAsync();
            return JsonConvert.DeserializeObject<List<ConfigurationClient>>(json, jsonSerializerSettings) ?? Enumerable.Empty<ConfigurationClient>();
        }

        /// <summary>
        /// Creates or updates client details in store
        /// </summary>
        /// <param name="client">Updated Client detsils</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        public async Task UpdateClientAsync(ConfigurationClient client)
        {
            var clients = await GetClientsAsync();
            var clientLookup = clients.ToDictionary(k => k.ClientId);
            clientLookup[client.ClientId] = client;
            await SaveClients(clientLookup.Values);
        }

        /// <summary>
        /// Creates or updates client group details in store
        /// </summary>
        /// <param name="clientGroup">Updated Client Group details</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        public async Task UpdateClientGroupAsync(ConfigurationClientGroup clientGroup)
        {
            var groups = await GetClientGroupsAsync();
            var clientLookup = groups.ToDictionary(k => k.GroupId);
            clientLookup[clientGroup.GroupId] = clientGroup;
            await SaveGroups(clientLookup.Values);
        }

        private async Task SaveClients(ICollection<ConfigurationClient> clients)
        {
            var json = JsonConvert.SerializeObject(clients, jsonSerializerSettings);
            await storageConnector.SetClientRegistryFileAsync(json);
        }

        private async Task SaveGroups(ICollection<ConfigurationClientGroup> clients)
        {
            var json = JsonConvert.SerializeObject(clients, jsonSerializerSettings);
            await storageConnector.SetClientGroupRegistryFileAsync(json);
        }
    }
}
