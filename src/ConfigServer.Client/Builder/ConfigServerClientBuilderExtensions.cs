﻿using ConfigServer.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Client
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
            source.Add(ServiceDescriptor.Transient<IConfigServer>(r => new ConfigServerClient(r.GetService<IHttpClientWrapper>(),r.GetService<IMemoryCache>(), r.GetService<ConfigurationRegistry>(), options)));
            return builder;
        }

        /// <summary>
        /// Adds ConfigInstance type to ConfigServer client registry
        /// </summary>
        /// <typeparam name="TConfig">ConfigInstance type to be added to registry</typeparam>
        /// <param name="source">Current ConfigServer client builder</param>
        /// <param name="name">Name of config on config server</param>
        /// <returns>ConfigServer client builder for further configuration</returns>
        public static ConfigServerClientBuilder WithConfig<TConfig>(this ConfigServerClientBuilder source, string name) where TConfig : class, new()
        {
            source.ConfigurationRegistry.AddRegistration(ConfigurationRegistration.Build<TConfig>(name));
            return source;
        }

        /// <summary>
        /// Adds ConfigInstance type to ConfigServer client registry
        /// </summary>
        /// <typeparam name="TConfig">ConfigInstance type to be added to registry</typeparam>
        /// <param name="source">Current ConfigServer client builder</param>
        /// <param name="name">Name of config on config server</param>
        /// <returns>ConfigServer client builder for further configuration</returns>
        public static ConfigServerClientBuilder WithCollectionConfig<TConfig>(this ConfigServerClientBuilder source, string name) where TConfig : class, new()
        {
            source.ConfigurationRegistry.AddRegistration(ConfigurationRegistration.BuildCollection<TConfig>(name));
            return source;
        }
    }
}
