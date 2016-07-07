using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Core
{
    public static class ConfigServerClientBuilderExtensions
    {
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
