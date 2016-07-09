using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.InMemoryProvider
{
    /// <summary>
    /// In memory implementation of IConfigRepository
    /// </summary>
    public class InMemoryRepository : IConfigRepository
    {
        private readonly Dictionary<string, Dictionary<Type, Config>> innerStore;
        
        /// <summary>
        /// Initializes InMemoryRepository
        /// </summary>
        public InMemoryRepository()
        {
            innerStore = new Dictionary<string, Dictionary<Type, Config>>();
        }

        /// <summary>
        /// Get all Client Ids in store
        /// </summary>
        /// <returns>AvailableClientIds</returns>
        public Task<IEnumerable<string>> GetClientIdsAsync()
        {
            var tcs = new TaskCompletionSource<IEnumerable<string>>();
            tcs.SetResult(GetConfigSetIds());
            return tcs.Task;
        }

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Config of the type requested</returns>
        public Task<Config> GetAsync(Type type, ConfigurationIdentity id)
        {
            var tcs = new TaskCompletionSource<Config>();
            tcs.SetResult(Get(type, id));
            return tcs.Task;
        }

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Config of the type requested</returns>
        public Task<Config<TConfig>> GetAsync<TConfig>(ConfigurationIdentity id) where TConfig : class, new()
        {
            var tcs = new TaskCompletionSource<Config<TConfig>>();
            tcs.SetResult(Get<TConfig>(id));
            return tcs.Task;
        }

        /// <summary>
        /// Saves changes to configuration
        /// </summary>
        /// <param name="config">Updated configuration to be saved</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public Task SaveChangesAsync(Config config)
        {
            var tcs = new TaskCompletionSource<bool>();
            SaveChanges(config);
            tcs.SetResult(true);
            return tcs.Task;
        }

        /// <summary>
        /// Creates new client in store
        /// </summary>
        /// <param name="clientId">new client Id</param>
        /// <returns>A task that represents the asynchronous creation operation.</returns>
        public Task CreateClientAsync(string clientId)
        {
            var tcs = new TaskCompletionSource<bool>();
            CreateConfigSet(clientId);
            tcs.SetResult(true);
            return tcs.Task;
        }

        private Config Get(Type type, ConfigurationIdentity id)
        {
            var innerDic = innerStore[id.ClientId];
            Config config;
            if (!innerDic.TryGetValue(type, out config))
            {
                config = ConfigFactory.CreateGenericInstance(type, id.ClientId);
            }

            return config;
        }

        private Config<TConfig> Get<TConfig>(ConfigurationIdentity id) where TConfig : class, new()
        {
            return (Config<TConfig>)Get(typeof(TConfig), id);
        }

        private IEnumerable<string> GetConfigSetIds()
        {
            return innerStore.Keys.ToList();
        }

        private void SaveChanges(Config config)
        {
            if (!innerStore.ContainsKey(config.ClientId))
                innerStore.Add(config.ClientId, new Dictionary<Type, Config>());

            innerStore[config.ClientId][config.ConfigType] = config;
        }

        private void CreateConfigSet(string configSetId)
        {
            innerStore.Add(configSetId, new Dictionary<Type, Config>());
        }

    }
}
