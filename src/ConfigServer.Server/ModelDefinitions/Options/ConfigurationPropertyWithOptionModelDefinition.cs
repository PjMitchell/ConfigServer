using ConfigServer.Core;
using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ConfigurationPropertyWithOptionModelDefinition<TConfigurationSet, TOption> : ConfigurationPropertyWithOptionModelDefinition where TConfigurationSet : ConfigurationSet
    {
        readonly IConfigurationSetOptionProvider<TConfigurationSet, TOption> optionProvider;
        readonly ConfigurationDependency[] dependency;

        public ConfigurationPropertyWithOptionModelDefinition(IConfigurationSetOptionProvider<TConfigurationSet, TOption> optionProvider, string propertyName, Type propertyParentType) : base(propertyName, typeof(TConfigurationSet), typeof(TOption), propertyParentType)
        {
            this.optionProvider = optionProvider;
            dependency = new[] { new ConfigurationDependency(typeof(TConfigurationSet), optionProvider.OptionPropertyName) };
        }

        public override IEnumerable<ConfigurationDependency> GetDependencies() => dependency;

        public override IOptionSet GetOptionSet(object configurationSet)
        {
            var castedConfigurationSet = (TConfigurationSet)configurationSet;
            return optionProvider.GetOptions(castedConfigurationSet);
        }

    }

    internal abstract class ConfigurationPropertyWithOptionModelDefinition : ConfigurationPropertyModelBase, IOptionPropertyDefinition
    {
        public ConfigurationPropertyWithOptionModelDefinition(string propertyName,Type configurationSet, Type optionType, Type propertyParentType) : base(propertyName, optionType, propertyParentType)
        {
            ConfigurationSetType = configurationSet;
        }

        public Type ConfigurationSetType { get; }

        public abstract IOptionSet GetOptionSet(object configurationSet);

    }
}
