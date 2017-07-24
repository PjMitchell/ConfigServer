using Microsoft.Extensions.DependencyInjection;
using ConfigServer.Core;
using ConfigServer.Server;

namespace ConfigServer.InMemoryProvider
{
    /// <summary>
    /// ConfigServer builder extensions for InMemoryProvider.
    /// Used in the initial configuration of ConfigServer InMemoryProvider
    /// </summary>
    public static class InMemoryBuilderExtensions
    {
        /// <summary>
        /// Uses InMemoryProvider as IConfigRepository  
        /// </summary>
        /// <param name="builder">ConfigServerBuilder to add InMemoryProvider to</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseInMemoryProvider(this ConfigServerBuilder builder)
        {
            var repo = new InMemoryRepository();
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton<IConfigClientRepository>(repo));
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton<IConfigRepository>(repo));
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton<IConfigProvider>(repo));
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton<IConfigurationSnapshotRepository>(new InMemorySnapshotRepository()));
            return builder;
        }
    }
}
