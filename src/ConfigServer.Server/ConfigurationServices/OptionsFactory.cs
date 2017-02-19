using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Server
{
    internal interface IOptionSetFactory
    {
        /// <summary>
        /// Builds OptionSet for Definition
        /// </summary>
        /// <param name="definition">Definition used to build optionSet</param>
        /// <param name="configIdentity">Identity of Configuration instance being loaded</param>
        /// <returns>OptionSet </returns>
        IOptionSet Build(ConfigurationPropertyWithOptionsModelDefinition definition, ConfigurationIdentity configIdentity);

        /// <summary>
        /// Builds OptionSet for Definition
        /// </summary>
        /// <param name="definition">Definition used to build optionSet</param>
        /// <param name="configurationSets">Configurations sets used to build optionSet</param>
        /// <returns>OptionSet </returns>
        IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition, IEnumerable<ConfigurationSet> configurationSets);

        /// <summary>
        /// Builds OptionSet for Definition
        /// </summary>
        /// <param name="definition">Definition used to build optionSet</param>
        /// <param name="configurationSet">Configuration set used to build optionSet</param>
        /// <returns>OptionSet </returns>
        IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition, ConfigurationSet configurationSet);

        /// <summary>
        /// Gets Key from Object
        /// </summary>
        /// <param name="value">object value</param>
        /// <param name="definition">Definition for option</param>
        /// <returns>Key for value</returns>
        string GetKeyFromObject(object value, ConfigurationPropertyWithConfigSetOptionsModelDefinition definition);
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

        public IOptionSet Build(ConfigurationPropertyWithOptionsModelDefinition definition, ConfigurationIdentity configIdentity)
        {
            return definition.BuildOptionSet(serviceProvider, configIdentity);
        }

        public IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition, IEnumerable<ConfigurationSet> configurationSets)
        {
            var configurationSet = configurationSets.Single(r => r.GetType() == definition.ConfigurationSetType);
            return Build(definition, configurationSet);
        }

        public IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition, ConfigurationSet configurationSet) => definition.GetOptionSet(configurationSet);

        public string GetKeyFromObject(object value, ConfigurationPropertyWithConfigSetOptionsModelDefinition definition)
        {
            var dependency = definition.GetDependencies().Single();
            var optionDefinition = (ConfigurationOptionModel)registry.GetConfigSetDefinition(dependency.ConfigurationSet).Get(definition.PropertyType);
            return optionDefinition.GetKeyFromObject(value);
        }
    }
}
