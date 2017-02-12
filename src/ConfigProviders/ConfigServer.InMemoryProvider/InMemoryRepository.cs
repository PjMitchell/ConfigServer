using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

namespace ConfigServer.InMemoryProvider
{
    /// <summary>
    /// In memory implementation of IConfigRepository
    /// </summary>
    public class InMemoryRepository : IConfigRepository
    {
        private readonly Dictionary<string, ConfigurationClient> clientStore;
        private readonly Dictionary<string, Dictionary<Type, ConfigInstance>> innerStore;
        
        /// <summary>
        /// Initializes InMemoryRepository
        /// </summary>
        public InMemoryRepository()
        {
            clientStore = new Dictionary<string, ConfigurationClient>();
            innerStore = new Dictionary<string, Dictionary<Type, ConfigInstance>>();
        }

        /// <summary>
        /// Get all Client Ids in store
        /// </summary>
        /// <returns>AvailableClientIds</returns>
        public Task<IEnumerable<ConfigurationClient>> GetClientsAsync()
        {
            var tcs = new TaskCompletionSource<IEnumerable<ConfigurationClient>>();
            tcs.SetResult(GetClients());
            return tcs.Task;
        }

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>ConfigInstance of the type requested</returns>
        public Task<ConfigInstance> GetAsync(Type type, ConfigurationIdentity id)
        {
            var tcs = new TaskCompletionSource<ConfigInstance>();
            tcs.SetResult(Get(type, id));
            return tcs.Task;
        }

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>ConfigInstance of the type requested</returns>
        public Task<ConfigInstance<TConfig>> GetAsync<TConfig>(ConfigurationIdentity id) where TConfig : class, new()
        {
            var tcs = new TaskCompletionSource<ConfigInstance<TConfig>>();
            tcs.SetResult(Get<TConfig>(id));
            return tcs.Task;
        }

        /// <summary>
        /// Gets Collection Configuration
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Enumerable of the type requested</returns>
        public Task<IEnumerable<TConfig>> GetCollectionAsync<TConfig>(ConfigurationIdentity id)
        {
            var item = Get<List<TConfig>>(id).Configuration;
            return Task.FromResult<IEnumerable<TConfig>>(item);
        }

        /// <summary>
        /// Gets Collection Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Enumerable of the type requested</returns>
        public Task<IEnumerable> GetCollectionAsync(Type type, ConfigurationIdentity id)
        {
            var config = typeof(List<>);
            Type[] typeArgs = { type };
            var configType = config.MakeGenericType(typeArgs);
            var item = (IEnumerable)Get(configType, id).GetConfiguration();
            return Task.FromResult(item);
        }

        /// <summary>
        /// Saves changes to configuration
        /// </summary>
        /// <param name="config">Updated configuration to be saved</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public Task UpdateConfigAsync(ConfigInstance config)
        {
            var tcs = new TaskCompletionSource<bool>();
            SaveChanges(config);
            tcs.SetResult(true);
            return tcs.Task;
        }

        /// <summary>
        /// Creates or updates client details in store
        /// </summary>
        /// <param name="client">Updated Client detsils</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        public Task UpdateClientAsync(ConfigurationClient client)
        {
            var tcs = new TaskCompletionSource<bool>();
            if (clientStore.ContainsKey(client.ClientId))
                clientStore[client.ClientId] = client;
            else
                CreateConfigSet(client);
            tcs.SetResult(true);
            return tcs.Task;
        }



        private ConfigInstance Get(Type type, ConfigurationIdentity id)
        {
            var innerDic = innerStore[id.ClientId];
            ConfigInstance config;
            if (!innerDic.TryGetValue(type, out config))
            {
                config = ConfigFactory.CreateGenericInstance(type, id.ClientId);
            }

            return config;
        }

        private ConfigInstance<TConfig> Get<TConfig>(ConfigurationIdentity id) where TConfig : class, new()
        {
            return (ConfigInstance<TConfig>)Get(typeof(TConfig), id);
        }

        private IEnumerable<ConfigurationClient> GetClients()
        {
            return clientStore.Values.ToList();
        }

        private void SaveChanges(ConfigInstance config)
        {
            if (!innerStore.ContainsKey(config.ClientId))
                innerStore.Add(config.ClientId, new Dictionary<Type, ConfigInstance>());

            innerStore[config.ClientId][config.ConfigType] = config;
        }

        private void CreateConfigSet(ConfigurationClient client)
        {
            clientStore.Add(client.ClientId, client);
            innerStore.Add(client.ClientId, new Dictionary<Type, ConfigInstance>());
        }


    }
}
