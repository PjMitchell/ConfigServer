using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.InMemoryProvider
{
    public class InMemoryRepository : IConfigRepository
    {
        private readonly Dictionary<Type, Dictionary<string, Config>> innerStore;

        public InMemoryRepository()
        {
            innerStore = new Dictionary<Type, Dictionary<string, Config>>();
        }

        public Config Get(Type type, ConfigurationIdentity id)
        {
            return innerStore[type][id.ApplicationIdentity];
        }

        public Config<TConfig> Get<TConfig>(ConfigurationIdentity id) where TConfig : class, new()
        {
            return (Config<TConfig>)innerStore[typeof(TConfig)][id.ApplicationIdentity];
        }


        public IEnumerable<string> GetApplicationIds()
        {
            return innerStore.SelectMany(s=> s.Value.Keys).Distinct().ToList();
        }

        public Task<IEnumerable<string>> GetApplicationIdsAsync()
        {
            var tcs = new TaskCompletionSource<IEnumerable<string>>();
            tcs.SetResult(GetApplicationIds());
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
            if (!innerStore.ContainsKey(config.ConfigType))
                innerStore.Add(config.ConfigType, new Dictionary<string, Config>());

            innerStore[config.ConfigType][config.ApplicationIdentity] = config;
        }

        public Task SaveChangesAsync(Config config)
        {
            var tcs = new TaskCompletionSource<bool>();
            SaveChanges(config);
            tcs.SetResult(true);
            return tcs.Task;
        }
    }
}
