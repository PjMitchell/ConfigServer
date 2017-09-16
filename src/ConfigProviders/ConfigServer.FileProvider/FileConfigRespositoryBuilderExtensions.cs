using ConfigServer.Core;
using ConfigServer.TextProvider.Core;
using ConfigServer.Server;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConfigServer.FileProvider
{
    /// <summary>
    /// ConfigServer builder extensions for FileConfigRepository.
    /// Used in the initial configuration of ConfigServer FileConfigRepository
    /// </summary>
    public static class FileConfigRespositoryBuilderExtensions
    {
        /// <summary>
        /// Uses FileConfigRepository as IConfigRepository  
        /// </summary>
        /// <param name="builder">ConfigServerBuilder to add FileConfigRepository to</param>
        /// <param name="options">Options for FileConfigRepository</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseFileConfigProvider(this ConfigServerBuilder builder, FileConfigRespositoryBuilderOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.ConfigStorePath))
                throw new ArgumentException($"{nameof(FileConfigRespositoryBuilderOptions.ConfigStorePath)} cannot be null or whitespace", nameof(options));
            builder.ServiceCollection.AddSingleton(options);
            builder.ServiceCollection.AddTransient<IConfigRepository, TextStorageConfigurationRepository>();
            builder.ServiceCollection.AddTransient<IConfigClientRepository, TextStorageConfigurationClientRepository>();
            builder.ServiceCollection.AddTransient<IConfigProvider, TextStorageConfigurationRepository>();
            builder.ServiceCollection.AddTransient<IStorageConnector, FileStorageConnector>();
            builder.ServiceCollection.AddTransient<IConfigArchive, FileConfigArchive>();
            builder.ServiceCollection.AddTransient<IConfigurationSnapshotRepository, TextStorageSnapshotRepository>();
            builder.ServiceCollection.AddTransient<ISnapshotStorageConnector, FileSnapshotStorageConnector>();


            return builder;
        }
        /// <summary>
        /// Uses FileResourceRepository as IConfigRepository  
        /// </summary>
        /// <param name="builder">ConfigServerBuilder to add FileResourceRepository to</param>
        /// <param name="options">Options for FileResourceRepository</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseFileResourceProvider(this ConfigServerBuilder builder, FileResourceRepositoryBuilderOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.ResourceStorePath))
                throw new ArgumentException($"{nameof(FileResourceRepositoryBuilderOptions.ResourceStorePath)} cannot be null or whitespace", nameof(options));
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton(options));
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IResourceStore, FileResourceStore>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IResourceArchive, FileResourceArchive>());

            return builder;
        }

    }
}
