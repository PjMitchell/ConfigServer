using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Server
{
    internal interface IOptionSetFactory
    {

        /// <summary>
        /// Builds OptionSet for Definition
        /// </summary>
        /// <param name="definition">Definition used to build optionSet</param>
        /// <param name="configIdentity">Identity of Configuration instance being loaded</param>
        /// <param name="configurationSets">Configurations sets used to build optionSet</param>
        /// <returns>OptionSet </returns>
        IOptionSet Build(IOptionPropertyDefinition definition, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> configurationSets);

        IOptionSet Build(ReadOnlyConfigurationOptionModel model, ConfigurationIdentity configIdentity);

        /// <summary>
        /// Gets Key from Object
        /// </summary>
        /// <param name="value">object value</param>
        /// <param name="definition">Definition for option</param>
        /// <returns>Key for value</returns>
        string GetKeyFromObject(object value, IOptionPropertyDefinition definition);
    }

    internal class OptionSetFactory : IOptionSetFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ConfigurationModelRegistry registry;

        public OptionSetFactory(IServiceProvider serviceProvider, ConfigurationModelRegistry registry)
        {
            this.serviceProvider = serviceProvider;
            this.registry = registry;
        }

        public IOptionSet Build(ReadOnlyConfigurationOptionModel model, ConfigurationIdentity configIdentity)
        {
            var optionProvider = model.OptionProviderType != null
                ? serviceProvider.GetService(model.OptionProviderType)
                : null;
            return model.BuildOptionSet(configIdentity, optionProvider);
        }

        public IOptionSet Build(IOptionPropertyDefinition definition, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> configurationSets)
        {
            return Build(definition, configurationSets);
        }

        public string GetKeyFromObject(object value, IOptionPropertyDefinition definition)
        {
            var dependency = definition.GetDependencies().Single();
            var optionModel = (ConfigurationOptionModel)registry.GetConfigSetDefinition(dependency.ConfigurationSet).Get(definition.PropertyType);
            return optionModel.GetKeyFromObject(value);
        }

        private IOptionSet Build(IOptionPropertyDefinition definition, IEnumerable<ConfigurationSet> configurationSets)
        {
            var configurationSet = configurationSets.Single(r => r.GetType() == definition.ConfigurationSetType);
            return Build(definition, configurationSet);
        }

        private IOptionSet Build(IOptionPropertyDefinition definition, ConfigurationSet configurationSet) => definition.GetOptionSet(configurationSet);
    }
}
