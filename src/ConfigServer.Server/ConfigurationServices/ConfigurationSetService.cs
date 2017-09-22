using ConfigServer.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IConfigurationSetService
    {
        Task<ConfigurationSet> GetConfigurationSet(Type type, ConfigurationIdentity identity);
        Task HandleConfigurationUpdatedEvent(ConfigurationUpdatedEvent arg);
    }

    internal class ConfigurationSetService : IConfigurationSetService
    {
        readonly IConfigurationSetFactory factory;
        readonly ICachingStrategy cachingStrategy;
        readonly ConfigurationModelRegistry registry;
        private const string cachePrefix = "ConfigServer_ConfigurationSetService_";


        public ConfigurationSetService(IConfigurationSetFactory factory, ICachingStrategy cachingStrategy, ConfigurationModelRegistry registry)
        {
            this.factory = factory;
            this.cachingStrategy = cachingStrategy;
            this.registry = registry;
        }

        public Task<ConfigurationSet> GetConfigurationSet(Type type, ConfigurationIdentity identity)
        {
            var key = GetKey(type, identity);
            return cachingStrategy.GetOrCreateAsync(cachePrefix + key,() => GetConfigurationSetFromSource(type, identity));
        }

        public Task HandleConfigurationUpdatedEvent(ConfigurationUpdatedEvent arg) => ClearConfig(arg.ConfigurationType, arg.Identity);



        private async Task<ConfigurationSet> GetConfigurationSetFromSource(Type type, ConfigurationIdentity identity)
        {
            if (!registry.TryGetConfigSetDefinition(type, out ConfigurationSetModel model))
                throw new InvalidOperationException($"Could not find ConfigurationSet of type: {type} in regiistry");
            var requiredConfigurationSetTypes = model.GetDependencies().Select(s => s.ConfigurationSet)
                .Distinct()
                .Where(w => type != w)
                .ToArray();
            var requiredConfigurationSet = new ConfigurationSet[requiredConfigurationSetTypes.Length];
            for (var i = 0; i< requiredConfigurationSetTypes.Length; i++)
            {
                requiredConfigurationSet[i] = await GetConfigurationSetFromSource(requiredConfigurationSetTypes[i], identity);
            }
            return await factory.BuildConfigSet(type, identity, requiredConfigurationSet);
        }

        private string GetKey(Type type, ConfigurationIdentity identity) => $"{type.Name}_{identity.Client.ClientId}";

        private async Task ClearConfig(Type configurationtype, ConfigurationIdentity identity)
        {
            var setModel = registry.GetConfigSetForConfig(configurationtype);
            await ClearCache(setModel.ConfigSetType, identity);
            if (setModel.Get(configurationtype) is ConfigurationOptionModel model)
            {
                var dependencies = registry.SelectMany(s => s.Configs)
                    .Where(set => set.ConfigurationSetType != setModel.ConfigSetType && set.GetDependencies().Any(a => a.ConfigurationSet == setModel.ConfigSetType && a.PropertyPath == model.Name))
                    .ToArray();
                foreach (var dependency in dependencies)
                {
                    await ClearConfig(dependency.Type, identity);
                }
            }
        }

        private Task ClearCache(Type type, ConfigurationIdentity identity)
        {
            var key = GetKey(type, identity);
            return cachingStrategy.Remove(cachePrefix + key);
        }
    }
}
