using ConfigServer.Server;
using ConfigServer.Sample.Models;
using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;

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
            return definition.GetOptionSet(configurationSet);
        }
    }
}
