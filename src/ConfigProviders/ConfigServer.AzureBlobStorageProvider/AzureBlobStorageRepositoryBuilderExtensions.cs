using ConfigServer.Core;
using ConfigServer.Server;
using ConfigServer.TextProvider.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
            
            builder.ServiceCollection.AddMemoryCache();
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton(options));
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IStorageConnector, StorageConnector>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigRepository, TextStorageConfigurationRepository>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigProvider, TextStorageConfigurationRepository>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigClientRepository, TextStorageConfigurationClientRepository>());
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
            return builder;
        }
    }
}
