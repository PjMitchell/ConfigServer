using ConfigServer.Core.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Core
{
    public static class ConfigServerBuilderExtensions
    {
        public static ConfigServerBuilder UseConfigServer(this IServiceCollection source)
        {
            source.Add(ServiceDescriptor.Transient<IConfigHttpResponseFactory, ConfigHttpResponseFactory>());
            return new ConfigServerBuilder(source);
        }

        public static ConfigServerBuilder UseLocalConfigServer(this IServiceCollection source)
        {
            return new ConfigServerBuilder(source);
        }

        public static ConfigServerBuilder UseConfigSet<TConfigSet>(this ConfigServerBuilder source) where TConfigSet : ConfigurationSet, new()
        {
            var configSet = new TConfigSet();
            var definition = configSet.BuildModelDefinition();
            source.ConfigurationSetCollection.AddConfigurationSet(definition);
            return source;
        }

        public static ConfigServerClientBuilder UseLocalConfigServerClient(this ConfigServerBuilder source, string applicationId)
        {
            var configurationCollection = new ConfigurationCollection();
            var builder = new ConfigServerClientBuilder(source.ServiceCollection, configurationCollection);
            source.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigServerClient>(r => new LocalConfigServerClient(r.GetService<IConfigProvider>(), applicationId)));
            return builder;
        }

        public static IApplicationBuilder UseConfigServer(this IApplicationBuilder app, ConfigServerOptions options = null)
        {
            options = options ?? new ConfigServerOptions();
            app.Use((context, next) => ConfigServerHost.Setup(context, next, options));
            return app;
        }
    }
}
