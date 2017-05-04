using ConfigServer.Core;
using ConfigServer.Server;
using ConfigServer.TextProvider.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace ConfigServer.AzureTableStorageProvider
{
    /// <summary>
    /// Azure Table Storage Repository Builder Extensions
    /// </summary>
    public static class AzureTableStorageRepositoryBuilderExtensions
    {
        /// <summary>
        /// Uses AzureTableStorageRepository as IConfigRepository  
        /// </summary>
        /// <param name="builder">ConfigServerBuilder to add AzureTableStorageRepository to</param>
        /// <param name="options">Options for AzureTableStorageRepository</param>
        /// <returns>ConfigServer builder for further configuration</returns>
        public static ConfigServerBuilder UseAzureTableStorageProvider(this ConfigServerBuilder builder, AzureTableStorageRepositoryBuilderOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.Uri == null)
                throw new ArgumentException($"{nameof(options.Uri)} cannot be null", nameof(options));
            if (options.Credentials == null)
                throw new ArgumentException($"{nameof(options.Credentials)} cannot be null", nameof(options));
            
            builder.ServiceCollection.AddMemoryCache();
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton<AzureTableStorageRepositoryBuilderOptions>(options));
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IStorageConnector, StorageConnector>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigRepository, TextStorageConfigurationRepository>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigClientRepository, TextStorageConfigurationClientRepository>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigProvider, TextStorageConfigurationRepository>());
            return builder;
        }        
    }
}
