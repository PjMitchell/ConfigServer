using ConfigServer.Core;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IConfigurationClientService
    {
        Task<IEnumerable<ConfigurationClient>> GetClients();
        Task<IEnumerable<ConfigurationClientGroup>> GetGroups();
        Task<ConfigurationClient> GetClientOrDefault(string key);
        Task<ConfigurationClientGroup> GetClientGroupOrDefault(string key);
        void HandleClientGroupUpdated(string groupId);
        void HandleClientUpdated(string groupId);
    }

    internal class ConfigurationClientService : IConfigurationClientService
    {
        private readonly IConfigClientRepository clientRepo;
        private readonly IMemoryCache memoryCache;
        private const string clientCache = "ConfigServer_ConfigurationClientService_Client";
        private const string clientGroupCache = "ConfigServer_ConfigurationClientService_ClientGroup";


        public ConfigurationClientService(IConfigClientRepository clientRepo, IMemoryCache memoryCache)
        {
            this.clientRepo = clientRepo;
            this.memoryCache = memoryCache;
        }

        public async Task<ConfigurationClientGroup> GetClientGroupOrDefault(string key)
        {
            var lookup = await GetClientGroupLookup();
            if (!lookup.TryGetValue(key, out var result))
                return null;
            return result;
        }

        public async Task<ConfigurationClient> GetClientOrDefault(string key)
        {
            var lookup = await GetClientLookup();
            if (!lookup.TryGetValue(key, out var result))
                return null;
            return result;
        }

        public async Task<IEnumerable<ConfigurationClient>> GetClients()
        {
            var lookup = await GetClientLookup();
            return lookup.Values;
        }

        public async Task<IEnumerable<ConfigurationClientGroup>> GetGroups()
        {
            var lookup = await GetClientGroupLookup();
            return lookup.Values;
        }

        public void HandleClientGroupUpdated(string groupId)
        {
            memoryCache.CreateEntry(clientGroupCache);
        }

        public void HandleClientUpdated(string groupId)
        {
            memoryCache.CreateEntry(clientCache);
        }

        private Task<Dictionary<string, ConfigurationClient>> GetClientLookup()
        {
            return memoryCache.GetOrCreateAsync(clientCache, async entry =>
            {
                var clients = await clientRepo.GetClientsAsync();
                return clients.ToDictionary(k => k.ClientId, StringComparer.OrdinalIgnoreCase);
            });
        }

        private Task<Dictionary<string, ConfigurationClientGroup>> GetClientGroupLookup()
        {
            return memoryCache.GetOrCreateAsync(clientCache, async entry =>
            {
                var clientGroups = await clientRepo.GetClientGroupsAsync();
                return clientGroups.ToDictionary(k => k.GroupId, StringComparer.OrdinalIgnoreCase);
            });
        }


    }
}
