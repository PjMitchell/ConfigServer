using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Core
{
    /// <summary>
    /// ConfigServer client builder extensions.
    /// Used in the initial configuration of the ConfigServer client
    /// </summary>
    public static class ConfigServerClientBuilderExtensions
    {
        /// <summary>
        /// Adds ConfigServer client to specified ServiceCollection  
        /// </summary>
        /// <param name="source">The IServiceCollection to add ConfigServer client to</param>
        /// <param name="options">Options for ConfigServer client</param>
        /// <returns>ConfigServer client builder for further configuration</returns>
        public static ConfigServerClientBuilder AddConfigServerClient(this IServiceCollection source, ConfigServerClientOptions options)
        {
            var configurationCollection = new ConfigurationRegistry();
            var builder = new ConfigServerClientBuilder(source, configurationCollection);
            source.Add(ServiceDescriptor.Transient<IHttpClientWrapper>(r => new HttpClientWrapper(options.Authenticator)));
            source.Add(ServiceDescriptor.Transient<IConfigServerClient>(r => new ConfigServerClient(r.GetService<IHttpClientWrapper>(), r.GetService<ConfigurationRegistry>(), options)));
            return builder;
        }

        /// <summary>
        /// Adds Config type to ConfigServer client registry
        /// </summary>
        /// <typeparam name="TConfig">Config type to be added to registry</typeparam>
        /// <param name="source">Current ConfigServer client builder</param>
        /// <returns>ConfigServer client builder for further configuration</returns>
        public static ConfigServerClientBuilder WithConfig<TConfig>(this ConfigServerClientBuilder source) where TConfig : class, new()
        {
            source.ServiceCollection.Add(ServiceDescriptor.Transient(r => r.GetService<IConfigServerClient>().BuildConfigAsync<TConfig>().Result));
            source.ConfigurationRegistry.AddRegistration(ConfigurationRegistration.Build<TConfig>());
            return source;
        }
    }
}
