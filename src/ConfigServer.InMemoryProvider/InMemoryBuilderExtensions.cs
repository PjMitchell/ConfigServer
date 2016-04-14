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
            builder.ServiceCollection.Add(ServiceDescriptor.Instance<IConfigRepository>(repo));
            builder.ServiceCollection.Add(ServiceDescriptor.Instance<IConfigProvider>(repo));
            return builder;
        }
    }
}
