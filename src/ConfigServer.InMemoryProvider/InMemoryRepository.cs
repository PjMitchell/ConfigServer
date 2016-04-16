using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace ConfigServer.InMemoryProvider
{
    public class InMemoryRepository : IConfigRepository
    {
        private readonly Dictionary<string, Dictionary<Type, Config>> innerStore;
        private ConfigurationCollection configurationCollection;

        public InMemoryRepository(ConfigurationCollection configurationCollection)
        {
            this.configurationCollection = configurationCollection;
            innerStore = new Dictionary<string, Dictionary<Type, Config>>();
        }

        public Config Get(Type type, ConfigurationIdentity id)
        {
            var innerDic = innerStore[id.ConfigSetId];
            Config config;
            if(!innerDic.TryGetValue(type, out config))
            {
                config = configurationCollection.Get(type).InitializeConfig();
                config.ConfigSetId = id.ConfigSetId;
            }
                
            return config;
        }

        public Config<TConfig> Get<TConfig>(ConfigurationIdentity id) where TConfig : class, new()
        {
            return (Config<TConfig>)Get(typeof(TConfig),id);
        }


        public IEnumerable<string> GetConfigSetIds()
        {
            return innerStore.Keys.ToList();
        }

        public Task<IEnumerable<string>> GetConfigSetIdsAsync()
        {
            var tcs = new TaskCompletionSource<IEnumerable<string>>();
            tcs.SetResult(GetConfigSetIds());
            return tcs.Task;
        }

        public Task<Config> GetAsync(Type type, ConfigurationIdentity id)
        {
            var tcs = new TaskCompletionSource<Config>();
            tcs.SetResult(Get(type, id));
            return tcs.Task;
        }

        public Task<Config<TConfig>> GetAsync<TConfig>(ConfigurationIdentity id) where TConfig : class, new()
        {
            var tcs = new TaskCompletionSource<Config<TConfig>>();
            tcs.SetResult(Get<TConfig>(id));
            return tcs.Task;
        }

        public void SaveChanges(Config config)
        {
            if (!innerStore.ContainsKey(config.ConfigSetId))
                innerStore.Add(config.ConfigSetId, new Dictionary<Type, Config>());
            
            innerStore[config.ConfigSetId][config.ConfigType] = config;
        }

        public Task SaveChangesAsync(Config config)
        {
            var tcs = new TaskCompletionSource<bool>();
            SaveChanges(config);
            tcs.SetResult(true);
            return tcs.Task;
        }

        public void CreateConfigSet(string configSetId)
        {
            innerStore.Add(configSetId, new Dictionary<Type, Config>());
        }

        public Task CreateConfigSetAsync(string configSetId)
        {
            var tcs = new TaskCompletionSource<bool>();
            CreateConfigSet(configSetId);
            tcs.SetResult(true);
            return tcs.Task;
        }
    }
}
