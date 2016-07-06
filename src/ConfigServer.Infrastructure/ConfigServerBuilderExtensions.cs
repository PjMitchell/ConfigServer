using Microsoft.Extensions.DependencyInjection;
using ConfigServer.Core;
using ConfigServer.Core.Hosting;
using ConfigServer.Core.Client;

namespace ConfigServer.Infrastructure
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
            source.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigServerClient>(r => new LocalConfigServerClient(r.GetService<IConfigProvider>(),applicationId)));
            return builder;
        }

        public static ConfigServerClientBuilder UseConfigServerClient(this IServiceCollection source, ConfigServerClientOptions options)
        {
            var configurationCollection = new ConfigurationCollection();
            var builder = new ConfigServerClientBuilder(source, configurationCollection);
            source.Add(ServiceDescriptor.Transient<IHttpClientWrapper>(r => new HttpClientWrapper(options.Authenticator)));
            source.Add(ServiceDescriptor.Transient<IConfigServerClient>(r => new ConfigServerClient(r.GetService<IHttpClientWrapper>(), r.GetService<ConfigurationCollection>(), options)));
            return builder;
        }

        public static ConfigServerClientBuilder WithConfig<TConfig>(this ConfigServerClientBuilder source) where TConfig : class, new()
        {
            source.ServiceCollection.Add(ServiceDescriptor.Transient(r => r.GetService<IConfigServerClient>().BuildConfigAsync<TConfig>().Result));
            source.ConfigurationCollection.AddRegistration(ConfigurationRegistration.Build<TConfig>());
            return source;
        }
    }
}
