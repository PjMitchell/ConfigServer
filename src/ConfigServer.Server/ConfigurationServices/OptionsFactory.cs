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
        private readonly ConfigurationSetRegistry registry;

        public OptionSetFactory(IServiceProvider serviceProvider, ConfigurationSetRegistry registry)
        {
            this.serviceProvider = serviceProvider;
            this.registry = registry;
        }

        public IOptionSet Build(IOptionPropertyDefinition definition, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> configurationSets)
        {
            if (definition is ConfigurationPropertyWithConfigSetOptionsModelDefinition configSetOptionModelDefinition)
                return Build(configSetOptionModelDefinition, configurationSets);
            if (definition is ConfigurationPropertyWithOptionsModelDefinition configOptionModelDefinition)
                return Build(configOptionModelDefinition, configIdentity);
            throw new InvalidOperationException($"Could not build option set for definition type of {definition.GetType()}");
        }

        public string GetKeyFromObject(object value, IOptionPropertyDefinition definition)
        {
            if (definition is ConfigurationPropertyWithOptionsModelDefinition optionDefinition)
                return optionDefinition.GetKeyFromObject(value);

            var dependency = definition.GetDependencies().Single();
            var optionModel = (ConfigurationOptionModel)registry.GetConfigSetDefinition(dependency.ConfigurationSet).Get(definition.PropertyType);
            return optionModel.GetKeyFromObject(value);
        }


        private IOptionSet Build(ConfigurationPropertyWithOptionsModelDefinition definition, ConfigurationIdentity configIdentity) => definition.BuildOptionSet(serviceProvider, configIdentity);

        private IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition, IEnumerable<ConfigurationSet> configurationSets)
        {
            var configurationSet = configurationSets.Single(r => r.GetType() == definition.ConfigurationSetType);
            return Build(definition, configurationSet);
        }

        private IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition, ConfigurationSet configurationSet) => definition.GetOptionSet(configurationSet);
    }
}
