using ConfigServer.Core;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IConfigurationSetService
    {
        Task<ConfigurationSet> GetConfigurationSet(Type type, ConfigurationIdentity identity);
    }

    internal class ConfigurationSetService : IConfigurationSetService
    {
        readonly IConfigurationSetFactory factory;
        readonly IMemoryCache memoryCache;
        readonly ConfigurationSetRegistry registry;
        private const string cachePrefix = "ConfigServer_ConfigurationSetService_";


        public ConfigurationSetService(IConfigurationSetFactory factory, IMemoryCache memoryCache, ConfigurationSetRegistry registry)
        {
            this.factory = factory;
            this.memoryCache = memoryCache;
            this.registry = registry;
        }

        public Task<ConfigurationSet> GetConfigurationSet(Type type, ConfigurationIdentity identity)
        {
            var key = GetKey(type, identity);

            return memoryCache.GetOrCreateAsync(cachePrefix + key, e => {
                e.SetSlidingExpiration(TimeSpan.FromMinutes(5));
                return GetConfigurationSetFromSource(type, identity);
            });
        }

        private async Task<ConfigurationSet> GetConfigurationSetFromSource(Type type, ConfigurationIdentity identity)
        {
            ConfigurationSetModel model;
            if (!registry.TryGetConfigSetDefinition(type, out model))
                throw new InvalidOperationException($"Could not find ConfigurationSet of type: {type} in regiistry");
            var requiredConfigurationSetTypes = model.GetDependencies().Select(s => s.ConfigurationSet)
                .Distinct()
                .Where(w => type != w)
                .ToArray();
            var requiredConfigurationSet = new ConfigurationSet[requiredConfigurationSetTypes.Length];
            for (var i = 0; i< requiredConfigurationSetTypes.Length; i++)
            {
                requiredConfigurationSet[i] = await GetConfigurationSetFromSource(requiredConfigurationSetTypes[i], identity);
            }
            return await factory.BuildConfigSet(type, identity, requiredConfigurationSet);
        }

        private string GetKey(Type type, ConfigurationIdentity identity) => $"{type.Name}_{identity.ClientId}";
    }
}
