using ConfigServer.Core;
using ConfigServer.Server;
using ConfigServer.TextProvider.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConfigServer.AzureBlobStorageProvider
{
    /// <summary>
    /// Azure Blob Storage Repository Builder Extensions
    /// </summary>
    public static class AzureBlobStorageRepositoryBuilderExtensions
    {
        /// <summary>
        /// Uses AzureBlobStorageRepository as IConfigRepository  
        /// </summary>
        /// <param name="builder">ConfigServerBuilder to add AzureBlobStorageRepository to</param>
        /// <param name="options">Options for AzureBlobStorageRepository</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseAzureBlobStorageConfigProvider(this ConfigServerBuilder builder, AzureBlobStorageRepositoryBuilderOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.Uri == null)
                throw new ArgumentException($"{nameof(options.Uri)} cannot be null", nameof(options));
            if (options.Credentials == null)
                throw new ArgumentException($"{nameof(options.Credentials)} cannot be null", nameof(options));
            
            builder.ServiceCollection.AddSingleton(options);
            builder.ServiceCollection.AddTransient<IStorageConnector, StorageConnector>();
            builder.ServiceCollection.AddTransient<IConfigRepository, TextStorageConfigurationRepository>();
            builder.ServiceCollection.AddTransient<IConfigProvider, TextStorageConfigurationRepository>();
            builder.ServiceCollection.AddTransient<IConfigClientRepository, TextStorageConfigurationClientRepository>();
            builder.ServiceCollection.AddTransient<IConfigArchive, AzureBlobStorageConfigArchive>();
            builder.ServiceCollection.AddTransient<IConfigurationSnapshotRepository, TextStorageSnapshotRepository>();
            builder.ServiceCollection.AddTransient<ISnapshotStorageConnector, AzureBlobStorageSnapshotStorageConnector>();

            return builder;
        }

        /// <summary>
        /// Uses FileResourceRepository as IConfigRepository  
        /// </summary>
        /// <param name="builder">ConfigServerBuilder to add FileResourceRepository to</param>
        /// <param name="options">Options for FileResourceRepository</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseAzureBlobStorageResourceProvider(this ConfigServerBuilder builder, AzureBlobStorageResourceStoreOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton(options));
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IResourceStore, AzureBlobStorageResourceStore>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IResourceArchive, AzureBlobStorageResourceArchive>());

            return builder;
        }
    }
}
