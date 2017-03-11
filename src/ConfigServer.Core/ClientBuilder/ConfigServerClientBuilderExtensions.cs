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
