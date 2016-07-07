using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    public interface IConfigRepository : IConfigProvider
    {
        Task SaveChangesAsync(Config config);
        Task<IEnumerable<string>> GetConfigSetIdsAsync();
        Task CreateConfigSetAsync(string configSetId);

    }

    public interface IConfigProvider
    {
        Task<Config<TConfig>> GetAsync<TConfig>(ConfigurationIdentity id) where TConfig : class, new();
        Task<Config> GetAsync(Type type,ConfigurationIdentity id);
    }
}
