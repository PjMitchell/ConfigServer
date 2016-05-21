﻿using Microsoft.Extensions.DependencyInjection;
using ConfigServer.Core;
namespace ConfigServer.Infrastructure
{
    public static class ConfigServerBuilderExtensions
    {
        public static ConfigServerBuilder UseConfigServer(this IServiceCollection source)
        {
            var configurationCollection = new ConfigurationCollection();
            return new ConfigServerBuilder(source);
        }

        public static ConfigServerBuilder UseConfigSet<TConfigSet>(this ConfigServerBuilder source) where TConfigSet : ConfigurationSet, new()
        {
            var configSet = new TConfigSet();
            var definition = configSet.BuildModelDefinition();
            source.ConfigurationSetCollection.AddConfigurationSet(definition);
            return source;
        }

        public static ConfigServerClientBuilder UseLocalConfigServer(this ConfigServerBuilder source, string applicationId)
        {
            var configurationCollection = new ConfigurationCollection();
            var builder = new ConfigServerClientBuilder(source.ServiceCollection, configurationCollection);
            source.ServiceCollection.Add(ServiceDescriptor.Transient<IConfigServer>(r => new LocalConfigServer(r.GetService<IConfigProvider>(),applicationId)));
            return builder;
        }

        public static ConfigServerClientBuilder WithConfig<TConfig>(this ConfigServerClientBuilder source) where TConfig : class, new()
        {
            source.ServiceCollection.Add(ServiceDescriptor.Transient(r => r.GetService<IConfigServer>().BuildConfig<TConfig>()));
            source.ConfigurationCollection.AddRegistration(ConfigurationRegistration.Build<TConfig>());
            return source;
        }
    }
}
