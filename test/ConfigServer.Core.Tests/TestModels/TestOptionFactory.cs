using ConfigServer.Server;
using ConfigServer.Sample.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ConfigServer.Core.Tests.TestModels
{
    internal class TestOptionSetFactory : IOptionSetFactory
    {
        public IOptionSet Build(ConfigurationPropertyWithOptionsModelDefinition definition, ConfigurationIdentity identity)
        {
            return new OptionSet<Option>(OptionProvider.Options, o => o.Id.ToString(), o => o.Description);
        }

        public IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition, IEnumerable<ConfigurationSet> configurationSets)
        {
            var configurationSet = configurationSets.SingleOrDefault(r => r.GetType() == definition.ConfigurationSetType);
            return Build(definition,configurationSet);
        }

        public IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition, ConfigurationSet configurationSet) => definition.GetOptionSet(configurationSet);

        public string GetKeyFromObject(object value, ConfigurationPropertyWithConfigSetOptionsModelDefinition definition)
        {
            var dynamic = (dynamic)value;
            return dynamic.Id.ToString();
        }
    }
}
