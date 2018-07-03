using ConfigServer.Server;
using System.Collections.Generic;
using System.Linq;
using System;
using ConfigServer.TestModels;

namespace ConfigServer.Core.Tests.TestModels
{
    internal class TestOptionSetFactory : IOptionSetFactory
    {
        public IOptionSet Build(ReadOnlyConfigurationOptionModel model, ConfigurationIdentity configIdentity)
        {
            return model.BuildOptionSet(configIdentity, new OptionProvider());
        }

        public IOptionSet Build(IOptionPropertyDefinition definition, ConfigurationSet configurationSet) => definition.GetOptionSet(configurationSet);

        public IOptionSet Build(IOptionPropertyDefinition definition, ConfigurationIdentity configIdentity, IEnumerable<ConfigurationSet> configurationSets)
        {
            var configurationSet = configurationSets.SingleOrDefault(r => r.GetType() == definition.ConfigurationSetType);
            return Build(definition, configurationSet);
        }



        public string GetKeyFromObject(object value, IOptionPropertyDefinition definition)
        {
            var dynamic = (dynamic)value;
            return dynamic.Id.ToString();
        }
    }
}
