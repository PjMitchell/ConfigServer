using ConfigServer.Core;
using ConfigServer.Server;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
            options.JsonSerializerSettings = options.JsonSerializerSettings ?? new JsonSerializerSettings();
            builder.ServiceCollection.AddMemoryCache();
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton<FileConfigRespositoryBuilderOptions>(options));
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigRepository, FileConfigRepository>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigProvider, FileConfigRepository>());
            return builder;
        }

    }
}
