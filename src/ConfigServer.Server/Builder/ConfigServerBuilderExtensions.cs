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
        /// Sets version of configuration service
        /// </summary>
        /// <param name="source">The ConfigServerBuilder to set the version for</param>
        /// <param name="version">Version of the configuration service. Helps to manage changes in the configuration sets</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder WithVersion(this ConfigServerBuilder source, Version version)
        {
            source.SetVersion(version);
            return source;
        }

        /// <summary>
        /// Adds a local ConfigServer to specified ServiceCollection with not capability of communicating over http   
        /// </summary>
        /// <param name="source">The IServiceCollection to add ConfigServer to</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseLocalConfigServer(this IServiceCollection source) => new ConfigServerBuilder(source);

        /// <summary>
        /// Adds ConfigSet to config model registry
        /// </summary>
        /// <typeparam name="TConfigSet">Type of config set to be added to registry</typeparam>
        /// <param name="source">The ConfigServerBuilder to add config set to</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseConfigSet<TConfigSet>(this ConfigServerBuilder source) where TConfigSet : ConfigurationSet<TConfigSet>, new()
        {
            var configSet = new TConfigSet();
            var definition = configSet.BuildConfigurationSetModel();
            source.AddConfigurationSet(definition);
            return source;
        }

        /// <summary>
        /// Adds CachingStrategy
        /// </summary>
        /// <typeparam name="TStrategy">Strategy to use</typeparam>
        /// <param name="source">The ConfigServerBuildern to add the strategy to</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseCachingStrategy<TStrategy>(this ConfigServerBuilder source) where TStrategy : class, ICachingStrategy
        {
            source.ServiceCollection.AddTransient<ICachingStrategy, TStrategy>();
            return source;
        }

        /// <summary>
        /// Uses InMemory Caching strategy
        /// </summary>
        /// <param name="source">The ConfigServerBuildern to add the strategy to</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseInMemoryCachingStrategy(this ConfigServerBuilder source) => source.UseCachingStrategy<MemoryCachingStrategy>();

        /// <summary>
        /// Uses Distributed Caching strategy
        /// </summary>
        /// <param name="source">The ConfigServerBuildern to add the strategy to</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseDistributedCachingStrategy(this ConfigServerBuilder source) => source.UseCachingStrategy<DistributedCachingStrategy>();

        /// <summary>
        /// Adds Local ConfigServer client to specified ServiceCollection  
        /// </summary>
        /// <param name="source">The ConfigServerBuilder to add local ConfigServer client to</param>
        /// <param name="applicationId">Identifier for application requesting the configuration</param>
        /// <param name="configServeruri">Identifier for application requesting the configuration</param>
        /// <returns>ConfigServer client builder for further configuration of client</returns>
        public static ConfigServerClientBuilder UseLocalConfigServerClient(this ConfigServerBuilder source, string applicationId, Uri configServeruri)
        {
            var configurationCollection = new ConfigurationRegistry();
            var builder = new ConfigServerClientBuilder(source.ServiceCollection, configurationCollection);
            source.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigServer>(r => new LocalConfigServerClient(r.GetService<IConfigProvider>(), r.GetService<IConfigurationClientService>(), r.GetService<IConfigurationModelRegistry>(), r.GetService<IResourceStore>(), applicationId, configServeruri)));
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
            
            app.MapEndpoint<ConfigManagerEndpoint>(HostPaths.Manager, options);
            app.MapEndpoint<ConfigClientEndPoint>(HostPaths.Clients, options);
            app.MapEndpoint<ConfigurationSetEndpoint>(HostPaths.ConfigurationSet, options);
            app.MapEndpoint<ConfigurationEditorEndpoint>(HostPaths.ConfigurationEditor, options);
            app.MapEndpoint<ConfigurationSetModelEndpoint>(HostPaths.ConfigurationSetModel, options);
            app.MapEndpoint<DownloadEndpoint>(HostPaths.Download, options);
            app.MapEndpoint<UploadEnpoint>(HostPaths.Upload, options);
            app.MapEndpoint<ResourceEndpoint>(HostPaths.Resource, options);
            app.MapEndpoint<ClientGroupEndpoint>(HostPaths.Group, options);
            app.MapEndpoint<GuidGeneratorEndpoint>(HostPaths.Guid, options);
            app.MapEndpoint<ResourceArchiveEndpoint>(HostPaths.ResourceArchive, options);
            app.MapEndpoint<ConfigArchiveEndPoint>(HostPaths.Archive, options);
            app.MapEndpoint<PermissionEndpoint>(HostPaths.UserPermissions, options);
            app.MapEndpoint<SnapshotEndpoint>(HostPaths.Snapshot, options);
            app.UseEndpoint<ConfigEndpoint>(options);
            
            return app;
        }
    }
}
