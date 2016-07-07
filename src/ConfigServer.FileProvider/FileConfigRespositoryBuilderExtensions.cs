using ConfigServer.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace ConfigServer.FileProvider
{
    public static class FileConfigRespositoryBuilderExtensions
    {
        public static ConfigServerBuilder UseInMemoryProvider(this ConfigServerBuilder builder, FileConfigRespositoryBuilderOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.ConfigStorePath))
                throw new ArgumentException($"{nameof(FileConfigRespositoryBuilderOptions.ConfigStorePath)} cannot be null or whitespace", nameof(options));
            options.JsonSerializerSettings = options.JsonSerializerSettings ?? new JsonSerializerSettings();
            
            builder.ServiceCollection.Add(ServiceDescriptor.Singleton<FileConfigRespositoryBuilderOptions>(options));
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigRepository, FileConfigRepository>());
            builder.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigProvider, FileConfigRepository>());
            return builder;
        }

    }
}
