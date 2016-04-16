using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ConfigServer.Core;
namespace ConfigServer.Infrastructure
{
    public static class ConfigServerBuilderExtensions
    {
        public static ConfigServerBuilder UseConfigServer(this IServiceCollection source)
        {
            var configurationCollection = new ConfigurationCollection();
            return new ConfigServerBuilder(source, configurationCollection);
        }

        public static ConfigServerBuilder WithConfig<TConfig>(this ConfigServerBuilder source) where TConfig : class, new()
        {
            source.ServiceCollection.Add(ServiceDescriptor.Transient(r => r.GetService<IConfigServer>().BuildConfig<TConfig>()));
            source.ConfigurationCollection.AddRegistration(ConfigurationRegistration.Build<TConfig>());
            return source;
        }

        public static ConfigServerBuilder UseLocalConfigServer(this ConfigServerBuilder source, string applicationId)
        {
            source.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigServer>(r => new LocalConfigServer(r.GetService<IConfigProvider>(),applicationId)));
            return source;
        }
    }
}
