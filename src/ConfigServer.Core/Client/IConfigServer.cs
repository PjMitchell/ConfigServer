using System;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    public interface IConfigServerClient
    {
        Task<TConfig> BuildConfigAsync<TConfig>() where TConfig : class, new();
        Task<object> BuildConfigAsync(Type type);
    }
}
