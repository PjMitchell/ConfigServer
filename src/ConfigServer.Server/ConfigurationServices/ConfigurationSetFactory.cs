using System;
using ConfigServer.Core;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal interface IConfigurationSetFactory
    {
        Task<ConfigurationSet> BuildConfigSet(Type type, ConfigurationIdentity identity, params ConfigurationSet[] requiredConfigurationSets);
        Task<ConfigurationSet<TConfigSet>> BuildConfigSet<TConfigSet>(ConfigurationIdentity identity, params ConfigurationSet[] requiredConfigurationSets) where TConfigSet : ConfigurationSet<TConfigSet>, new();
    }

    internal class ConfigurationSetFactory : IConfigurationSetFactory
    {
        private readonly IConfigProvider configProvider;
        private readonly IOptionSetFactory optionSetFactory;
        private readonly ConfigurationSetRegistry registry;

        public ConfigurationSetFactory(IConfigProvider configProvider, IOptionSetFactory optionSetFactory, ConfigurationSetRegistry registry)
        {
            this.configProvider = configProvider;
            this.optionSetFactory = optionSetFactory;
            this.registry = registry;
        }

        public async Task<ConfigurationSet> BuildConfigSet(Type type, ConfigurationIdentity identity, params ConfigurationSet[] requiredConfigurationSets)
        {
            var result = CreateGenericInstance(type, identity);
            
            var setDefinition = registry.GetConfigSetDefinition(type);
            await BuildOptions(result, setDefinition,type, requiredConfigurationSets, identity);
            await BuildConfigurations(result, setDefinition, type, requiredConfigurationSets, identity);

            return result;
        }

        public async Task<ConfigurationSet<TConfigSet>> BuildConfigSet<TConfigSet>(ConfigurationIdentity identity,params ConfigurationSet[] requiredConfigurationSets) where TConfigSet : ConfigurationSet<TConfigSet>, new()
        {
            var result = await BuildConfigSet(typeof(TConfigSet), identity);
            return (ConfigurationSet<TConfigSet>)result;
        }

        private async Task BuildOptions(ConfigurationSet result, ConfigurationSetModel setDefinition,Type setType, IEnumerable<ConfigurationSet> configurationSets, ConfigurationIdentity identity)
        {
            var configurationDependencies = configurationSets.Concat(new[] { result }).ToArray();
            foreach (var option in GetOptionsInOrder(setDefinition))
            {
                var options = await configProvider.GetCollectionAsync(option.Type, identity);

                var optionSet = option.BuildOptionSet(options);
                foreach(var item in optionSet.Values)
                {
                    UpdateOptions(item, option.ConfigurationProperties, configurationDependencies, identity);
                }
                setType.GetProperty(option.Name).SetValue(result,optionSet);
            }
        }

        private async Task BuildConfigurations(ConfigurationSet result, ConfigurationSetModel setDefinition, Type setType, IEnumerable<ConfigurationSet> configurationSets, ConfigurationIdentity identity)
        {
            var configurationDependencies = configurationSets.Concat(new[] { result }).ToArray();
            foreach (var configDefinition in setDefinition.Configs.Where(w=> !(w is ConfigurationOptionModel)))
            {
                var configInstance = await configProvider.GetAsync(configDefinition.Type, identity);
                var configuration = configInstance.GetConfiguration();
                UpdateOptions(configuration, configDefinition.ConfigurationProperties, configurationDependencies, identity);
                var configPayload = Config.Create(configDefinition.Type, configuration);
                setType.GetProperty(configDefinition.Name).SetValue(result, configPayload);
            }
        }

        #region Update Options Logic
        private void UpdateOptions(object source, Dictionary<string, ConfigurationPropertyModelBase> models, IEnumerable<ConfigurationSet> configurationSets, ConfigurationIdentity configIdentity)
        {
            foreach (var model in models)
            {
                UpdateOptions(source, model.Value, configurationSets, configIdentity);
            }
        }

        private void UpdateOptions(object source, ConfigurationPropertyModelBase model, IEnumerable<ConfigurationSet> configurationSets, ConfigurationIdentity configIdentity)
        {

            switch (model)
            {
                case ConfigurationPropertyWithMultipleOptionsModelDefinition optionsProperty:
                    UpdateOptions(source, optionsProperty, configIdentity);
                    break;
                case ConfigurationPropertyWithOptionsModelDefinition optionProperty:
                    UpdateOptions(source, optionProperty, configIdentity);
                    break;
                case ConfigurationCollectionPropertyDefinition collectionProperty:
                    UpdateOptions(source, collectionProperty, configurationSets, configIdentity);
                    break;
                case ConfigurationPropertyWithConfigSetOptionsModelDefinition collectionProperty:
                    UpdateOptions(source, collectionProperty, configurationSets, configIdentity);
                    break;
                default:
                    break;
            }
        }

        private void UpdateOptions(object source, ConfigurationPropertyWithConfigSetOptionsModelDefinition model, IEnumerable<ConfigurationSet> configurationSets, ConfigurationIdentity configIdentity)
        {

            var orignal = model.GetPropertyValue(source);
            if (orignal == null)
                return;
            var optionSet = optionSetFactory.Build(model, configurationSets);
            optionSet.TryGetValue(orignal, out var actualValue);
            model.SetPropertyValue(source, actualValue);
        }

        private void UpdateOptions(object source, ConfigurationPropertyWithOptionsModelDefinition model, ConfigurationIdentity configIdentity)
        {

            var orignal = model.GetPropertyValue(source);
            if (orignal == null)
                return;
            var optionSet = optionSetFactory.Build(model, configIdentity);
            optionSet.TryGetValue(orignal, out var actualValue);
            model.SetPropertyValue(source, actualValue);
        }

        private void UpdateOptions(object source, ConfigurationPropertyWithMultipleOptionsModelDefinition model, ConfigurationIdentity configIdentity)
        {
            var optionSet = optionSetFactory.Build(model, configIdentity);
            var collectionBuilder = model.GetCollectionBuilder();
            var items = model.GetPropertyValue(source) as IEnumerable;
            foreach (var item in items ?? Enumerable.Empty<object>())
            {
                if (optionSet.TryGetValue(item, out var actualValue))
                    collectionBuilder.Add(actualValue);
            }

            model.SetPropertyValue(source, collectionBuilder.Collection);
        }

        private void UpdateOptions(object source, ConfigurationCollectionPropertyDefinition model, IEnumerable<ConfigurationSet> configurationSets, ConfigurationIdentity configIdentity)
        {
            var items = model.GetPropertyValue(source) as IEnumerable;
            if (items == null)
            {
                var collectionBuilder = model.GetCollectionBuilder();
                model.SetPropertyValue(source, collectionBuilder.Collection);
                return;
            }
            foreach (var item in items)
            {
                UpdateOptions(item, model.ConfigurationProperties, configurationSets, configIdentity);
            }
        }

        #endregion

        private IEnumerable<ConfigurationOptionModel> GetOptionsInOrder(ConfigurationSetModel setDefinition)
        {
            var optionLookup = setDefinition.Configs.OfType<ConfigurationOptionModel>().ToDictionary(k => k.Name);
            var dependenciesLookup = optionLookup.ToDictionary(k => k.Key, v => v.Value.GetDependencies()
                .Where(w => w.ConfigurationSet == setDefinition.ConfigSetType)
                .Select(s => s.PropertyPath)
                .Distinct()
                .ToList());
            if (dependenciesLookup.Any(r => r.Value.Contains(r.Key)))
                throw new Exception("Circular Dependencies of Options Detected");
            var handledOption = new HashSet<string>();

            while (optionLookup.Count != 0)
            {
                var removedItems = new List<string>();
                foreach (var option in optionLookup)
                {
                    var dependencies = dependenciesLookup[option.Key];
                    dependencies.RemoveAll(handledOption.Contains);
                    if (dependencies.Count != 0)
                        continue;
                    handledOption.Add(option.Key);
                    removedItems.Add(option.Key);
                    yield return option.Value;
                }
                foreach (var item in removedItems)
                {
                    optionLookup.Remove(item);
                }
            }
        }

        private static ConfigurationSet CreateGenericInstance(Type type, ConfigurationIdentity identity)
        {
            var result = (ConfigurationSet)Activator.CreateInstance(type);
            result.Instance = identity;
            return result;
        }
    }
}
