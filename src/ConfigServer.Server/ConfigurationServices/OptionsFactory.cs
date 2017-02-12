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
        IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition,IEnumerable<ConfigurationSet> configurationSets);
    }

    internal class OptionSetFactory : IOptionSetFactory
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Instantiates a new OptionSetFactory
        /// </summary>
        /// <param name="serviceProvider">Service Provider that has services for option provider</param>
        public OptionSetFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IOptionSet Build(ConfigurationPropertyWithOptionsModelDefinition definition, ConfigurationIdentity configIdentity)
        {
            return definition.BuildOptionSet(serviceProvider, configIdentity);
        }

        public IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition, IEnumerable<ConfigurationSet> configurationSets)
        {
            var configurationSet = configurationSets.Single(r => r.GetType() == definition.ConfigurationSetType);
            return definition.GetOptionSet(configurationSet);
        }
    }
}
