using ConfigServer.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Reflection;

namespace ConfigServer.Server
{
    /// <summary>
    /// ConfigServer builder extensions.
    /// Used in the initial configuration of ConfigServer
    /// </summary>
    public static class ConfigServerBuilderExtensions
    {
        /// <summary>
        /// Adds ConfigServer to specified ServiceCollection  
        /// </summary>
        /// <param name="source">The IServiceCollection to add ConfigServer to</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder AddConfigServer(this IServiceCollection source)
        {
            source.AddConfigServerServices();
            return new ConfigServerBuilder(source);
        }

        /// <summary>
        /// Adds a local ConfigServer to specified ServiceCollection with not capability of communicating over http   
        /// </summary>
        /// <param name="source">The IServiceCollection to add ConfigServer to</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseLocalConfigServer(this IServiceCollection source)
        {
            return new ConfigServerBuilder(source);
        }

        /// <summary>
        /// Adds ConfigSet to config model registry
        /// </summary>
        /// <typeparam name="TConfigSet">Type of config set to be added to registry</typeparam>
        /// <param name="source">The IServiceCollection to add config set to</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseConfigSet<TConfigSet>(this ConfigServerBuilder source) where TConfigSet : ConfigurationSet<TConfigSet>, new()
        {
            var configSet = new TConfigSet();
            var definition = configSet.BuildConfigurationSetModel();
            source.ConfigurationSetCollection.AddConfigurationSet(definition);
            return source;
        }

        /// <summary>
        /// Adds Local ConfigServer client to specified ServiceCollection  
        /// </summary>
        /// <param name="source">The IServiceCollection to add local ConfigServer client to</param>
        /// <param name="applicationId">Identifier for application requesting the configuration</param>
        /// <param name="configServeruri">Identifier for application requesting the configuration</param>
        /// <returns>ConfigServer client builder for further configuration of client</returns>
        public static ConfigServerClientBuilder UseLocalConfigServerClient(this ConfigServerBuilder source, string applicationId, Uri configServeruri)
        {
            var configurationCollection = new ConfigurationRegistry();
            var builder = new ConfigServerClientBuilder(source.ServiceCollection, configurationCollection);
            source.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigServer>(r => new LocalConfigServerClient(r.GetService<IConfigProvider>(), r.GetService<IConfigurationClientService>(), r.GetService<IResourceStore>(), applicationId, configServeruri)));
            return builder;
        }

        /// <summary>
        /// Adds ConfigServer to the request pipeline
        /// </summary>
        /// <param name="app">The Microsoft.AspNetCore.Builder.IApplicationBuilder.</param>
        /// <param name="options">ConfigServer options</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseConfigServer(this IApplicationBuilder app, ConfigServerOptions options = null)
        {
            options = options ?? new ConfigServerOptions();
            var assembly = typeof(ConfigServerBuilderExtensions).GetTypeInfo().Assembly;
            var provider = new EmbeddedFileProvider(assembly);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = provider
            });
            
            app.Map(HostPaths.Manager, client => client.UseEndpoint<ConfigManagerEndpoint>(options));
            app.Map(HostPaths.Clients, client => client.UseEndpoint<ConfigClientEndPoint>(options));
            app.Map(HostPaths.ConfigurationSet, client => client.UseEndpoint<ConfigurationSetEnpoint>(options));
            app.Map(HostPaths.Download, client => client.UseEndpoint<DownloadEndpoint>(options));
            app.Map(HostPaths.Upload, client => client.UseEndpoint<UploadEnpoint>(options));
            app.Map(HostPaths.Resource, client => client.UseEndpoint<ResourceEndpoint>(options));
            app.Map(HostPaths.Group, client => client.UseEndpoint<ClientGroupEndpoint>(options));

            app.UseEndpoint<ConfigEnpoint>(options);
            
            return app;
        }
    }
}
