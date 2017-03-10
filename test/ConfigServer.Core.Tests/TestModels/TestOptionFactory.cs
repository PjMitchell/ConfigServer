using ConfigServer.Server;
using ConfigServer.Sample.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ConfigServer.Core.Tests.TestModels
{
    internal class TestOptionSetFactory : IOptionSetFactory
    {
        public IOptionSet Build(ReadOnlyConfigurationOptionModel model, ConfigurationIdentity configIdentity)
        {
            return model.BuildOptionSet(configIdentity, new OptionProvider());
        }

        public IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition, IEnumerable<ConfigurationSet> configurationSets)
        {
            var configurationSet = configurationSets.SingleOrDefault(r => r.GetType() == definition.ConfigurationSetType);
            return Build(definition,configurationSet);
        }

        public IOptionSet Build(ConfigurationPropertyWithConfigSetOptionsModelDefinition definition, ConfigurationSet configurationSet) => definition.GetOptionSet(configurationSet);

        public IOptionSet Build(IOptionPropertyDefinition definition, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> configurationSets)
        {
            if (definition is ConfigurationPropertyWithConfigSetOptionsModelDefinition configSetOptionModelDefinition)
                return Build(configSetOptionModelDefinition, configurationSets);
            throw new InvalidOperationException($"Could not build option set for definition type of {definition.GetType()}");
        }



        public string GetKeyFromObject(object value, IOptionPropertyDefinition definition)
        {
            var dynamic = (dynamic)value;
            return dynamic.Id.ToString();
        }
    }
}
