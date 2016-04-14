using System;

namespace ConfigServer.Core
{
    public interface IConfigServer
    {
        TConfig BuildConfig<TConfig>() where TConfig : class, new();
        object BuildConfig(Type type);
    }
}
