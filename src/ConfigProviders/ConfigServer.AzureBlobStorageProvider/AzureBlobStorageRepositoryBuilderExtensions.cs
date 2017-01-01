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
        public static ConfigServerBuilder UseFileConfigProvider(this ConfigServerBuilder builder, AzureBlobStorageRepositoryBuilderOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.Uri == null)
                throw new ArgumentException($"{nameof(options.Uri)} cannot be null", nameof(options));
            if (options.Credentials == null)
                throw new ArgumentException($"{nameof(options.Credentials)} cannot be null", nameof(options));

            options.JsonSerializerSettings = options.JsonSerializerSettings ?? new JsonSerializerSettings();
            builder.ServiceCollection.AddMemoryCache();
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton(options));
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton<ITextStorageSetting>(options));
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IStorageConnector, StorageConnector>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigRepository, TextStorageConfigurationRepository>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigProvider, TextStorageConfigurationRepository>());
            return builder;
        }        
    }
}
