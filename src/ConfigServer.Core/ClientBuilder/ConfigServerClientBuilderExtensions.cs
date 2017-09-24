using ConfigServer.Core;
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
        /// Sets configserver to use a single clientId to provide the configuration
        /// </summary>
        /// <param name="source">Current ConfigServer client builder</param>
        /// <param name="clientId">Client Id used to provide the configuration</param>
        /// <returns>ConfigServer client builder for further configuration</returns>
        public static ConfigServerClientBuilder WithClientId(this ConfigServerClientBuilder source, string clientId)
        {
            source.AddSingleton(new SingleClientIdProvider(clientId));
            return source;
        }

        /// <summary>
        /// Setup config servers ClientIdProvider as a transient service to be used to determine the clientId
        /// </summary>
        /// <typeparam name="TClientIdProvider">ClientIdProvider to be used in determining the client Id used</typeparam>
        /// <param name="source">Current ConfigServer client builder</param>
        /// <returns>ConfigServer client builder for further configuration</returns>
        public static ConfigServerClientBuilder WithClientIdProvider<TClientIdProvider>(this ConfigServerClientBuilder source) where TClientIdProvider : class, IClientIdProvider
        {
            source.AddTransient<IClientIdProvider, TClientIdProvider>();
            return source;
        }

        /// <summary>
        /// Adds ConfigInstance type to ConfigServer client registry
        /// </summary>
        /// <typeparam name="TConfig">ConfigInstance type to be added to registry</typeparam>
        /// <param name="source">Current ConfigServer client builder</param>
        /// <returns>ConfigServer client builder for further configuration</returns>
        public static ConfigServerClientBuilder WithConfig<TConfig>(this ConfigServerClientBuilder source) where TConfig : class, new()
        {
            source.ConfigurationRegistry.AddRegistration(ConfigurationRegistration.Build<TConfig>());
            return source;
        }

        /// <summary>
        /// Adds ConfigInstance type to ConfigServer client registry
        /// </summary>
        /// <typeparam name="TConfig">ConfigInstance type to be added to registry</typeparam>
        /// <param name="source">Current ConfigServer client builder</param>
        /// <returns>ConfigServer client builder for further configuration</returns>
        public static ConfigServerClientBuilder WithCollectionConfig<TConfig>(this ConfigServerClientBuilder source) where TConfig : class, new()
        {
            source.ConfigurationRegistry.AddRegistration(ConfigurationRegistration.BuildCollection<TConfig>());
            return source;
        }
    }
}
