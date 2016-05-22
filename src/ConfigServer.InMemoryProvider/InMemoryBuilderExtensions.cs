using ConfigServer.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ConfigServer.Core;

namespace ConfigServer.InMemoryProvider
{
    public static class InMemoryBuilderExtensions
    {
        public static ConfigServerBuilder UseInMemoryProvider(this ConfigServerBuilder builder)
        {
            var repo = new InMemoryRepository();
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton<IConfigRepository>(repo));
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton<IConfigProvider>(repo));
            return builder;
        }
    }
}
