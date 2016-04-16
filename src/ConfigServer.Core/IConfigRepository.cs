using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    public interface IConfigRepository : IConfigProvider
    {
        void SaveChanges(Config config);
        Task SaveChangesAsync(Config config);
        IEnumerable<string> GetConfigSetIds();
        Task<IEnumerable<string>> GetConfigSetIdsAsync();
        void CreateConfigSet(string configSetId);
        Task CreateConfigSetAsync(string configSetId);

    }

    public interface IConfigProvider
    {
        Config<TConfig> Get<TConfig>(ConfigurationIdentity id) where TConfig : class, new();
        Config Get(Type type, ConfigurationIdentity id);

        Task<Config<TConfig>> GetAsync<TConfig>(ConfigurationIdentity id) where TConfig : class, new();
        Task<Config> GetAsync(Type type,ConfigurationIdentity id);
    }
}
